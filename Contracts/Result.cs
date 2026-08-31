namespace Forno.Contracts;

public sealed class Result
{
    private Result(IReadOnlyList<FieldError> errors) => Errors = errors;

    public IReadOnlyList<FieldError> Errors { get; }

    public bool IsSuccess => Errors.Count == 0;

    public string? this[string field] =>
        Errors.FirstOrDefault(e => e.Field.Equals(field, StringComparison.OrdinalIgnoreCase))?.Message;

    public static Result Ok() => new([]);

    public static Result Fail(string field, string message) =>
        new([new FieldError(field, message)]);

    public static Result Fail(IEnumerable<FieldError> errors) =>
        new(errors.ToList());
}

public sealed class Result<T>
{
    private Result(T? value, IReadOnlyList<FieldError> errors)
    {
        Value = value;
        Errors = errors;
    }

    public T? Value { get; }

    public IReadOnlyList<FieldError> Errors { get; }

    public bool IsSuccess => Errors.Count == 0 && Value is not null;

    public string? this[string field] =>
        Errors.FirstOrDefault(e => e.Field.Equals(field, StringComparison.OrdinalIgnoreCase))?.Message;

    public static Result<T> Ok(T value) => new(value, []);

    public static Result<T> Fail(string field, string message) =>
        new(null, [new FieldError(field, message)]);

    public static Result<T> Fail(IEnumerable<FieldError> errors) =>
        new(null, errors.ToList());
}
