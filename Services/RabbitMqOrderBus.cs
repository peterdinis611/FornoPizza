using System.Text;
using System.Text.Json;
using Forno.Configuration;
using Forno.Data;
using Forno.Interfaces;
using Forno.Mapping;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Forno.Services;

public sealed class RabbitMqOrderBus : IOrderBus, IAsyncDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqOrderBus> _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _topologyReady;

    public RabbitMqOrderBus(IOptions<RabbitMqOptions> options, ILogger<RabbitMqOrderBus> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public bool IsEnabled => _options.IsConfigured;

    public async Task PublishKitchenTicketAsync(
        OvenOrder order,
        string eventName,
        CancellationToken cancellation = default)
    {
        if (!IsEnabled)
        {
            return;
        }

        try
        {
            await EnsureReadyAsync(cancellation);
            var ticket = OrderTicketMapper.ToTicket(order, eventName);
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ticket, JsonOptions));
            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = $"{order.Id}:{eventName}",
                Type = eventName
            };

            await _channel!.BasicPublishAsync(
                exchange: _options.Exchange,
                routingKey: _options.KitchenRoutingKey,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: cancellation);

            _logger.LogInformation(
                "RabbitMQ: lístok {OrderId} ({Event}) → {Queue}",
                order.Id,
                eventName,
                _options.KitchenQueue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ: nepodarilo sa poslať lístok {OrderId}", order.Id);
        }
    }

    private async Task EnsureReadyAsync(CancellationToken cancellation)
    {
        if (_topologyReady && _connection is { IsOpen: true } && _channel is { IsOpen: true })
        {
            return;
        }

        await _gate.WaitAsync(cancellation);
        try
        {
            if (_topologyReady && _connection is { IsOpen: true } && _channel is { IsOpen: true })
            {
                return;
            }

            await DisposeChannelAsync();

            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                AutomaticRecoveryEnabled = true
            };

            _connection = await factory.CreateConnectionAsync("forno-publisher", cancellation);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellation);

            await _channel.ExchangeDeclareAsync(
                exchange: _options.Exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellation);

            await _channel.QueueDeclareAsync(
                queue: _options.KitchenQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellation);

            await _channel.QueueBindAsync(
                queue: _options.KitchenQueue,
                exchange: _options.Exchange,
                routingKey: _options.KitchenRoutingKey,
                cancellationToken: cancellation);

            _topologyReady = true;
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task DisposeChannelAsync()
    {
        _topologyReady = false;
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
            _channel = null;
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _gate.WaitAsync();
        try
        {
            await DisposeChannelAsync();
        }
        finally
        {
            _gate.Release();
            _gate.Dispose();
        }
    }
}
