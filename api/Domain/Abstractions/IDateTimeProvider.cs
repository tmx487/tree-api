namespace api.Domain.Abstractions;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}