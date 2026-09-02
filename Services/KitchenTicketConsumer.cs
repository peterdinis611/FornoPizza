using System.Text;
using System.Text.Json;
using Forno.Configuration;
using Forno.Contracts;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Forno.Services;

public sealed class KitchenTicketConsumer(
    IOptions<RabbitMqOptions> options,
    ILogger<KitchenTicketConsumer> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly RabbitMqOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.IsConfigured)
        {
            logger.LogInformation("RabbitMQ: kitchen consumer vypnutý (RabbitMq:Enabled=false).");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConsumeLoopAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "RabbitMQ: kitchen consumer sa odpojil, skúsim znova o 5s.");
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private async Task ConsumeLoopAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            AutomaticRecoveryEnabled = true
        };

        await using var connection = await factory.CreateConnectionAsync("forno-kitchen", stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(
            exchange: _options.Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: _options.KitchenQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(
            queue: _options.KitchenQueue,
            exchange: _options.Exchange,
            routingKey: _options.KitchenRoutingKey,
            cancellationToken: stoppingToken);

        await channel.BasicQosAsync(0, 1, false, stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var ticket = JsonSerializer.Deserialize<OrderTicketMessage>(json, JsonOptions);
                if (ticket is null)
                {
                    logger.LogWarning("RabbitMQ: neplatný lístok, ACK.");
                    await channel.BasicAckAsync(args.DeliveryTag, false, stoppingToken);
                    return;
                }

                var lines = string.Join(", ", ticket.Lines.Select(l =>
                    $"{l.Quantity}× {l.PizzaName}" + (string.IsNullOrWhiteSpace(l.Extras) ? "" : $" (+{l.Extras})")));

                logger.LogInformation(
                    "Kuchyňa ← #{OrderId} · {Event} · {Name} · {Fulfillment} · {Total:N2} € · {Lines}",
                    ticket.OrderId,
                    ticket.Event,
                    ticket.Name,
                    ticket.Fulfillment,
                    ticket.Total,
                    lines);

                await channel.BasicAckAsync(args.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RabbitMQ: chyba pri spracovaní lístka.");
                await channel.BasicNackAsync(args.DeliveryTag, false, requeue: true, cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: _options.KitchenQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        logger.LogInformation("RabbitMQ: kitchen consumer počúva frontu {Queue}.", _options.KitchenQueue);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
        }
    }
}
