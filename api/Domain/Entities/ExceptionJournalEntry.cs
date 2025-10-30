using api.Domain.Abstractions;

namespace api.Domain;

/// <summary>
/// Represents a journal entry for exceptions that occurred during API request processing.
/// </summary>
public class ExceptionJournalEntry
{
    public long Id { get; private set; }

    public long EventId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public string ExceptionType { get; private set; }

    public string Message { get; private set; }

    public string StackTrace { get; private set; }

    public string RequestPath { get; private set; }

    public string HttpMethod { get; private set; }

    public string QueryParameters { get; private set; }

    public string BodyParameters { get; private set; }

    private ExceptionJournalEntry()
    {
    }

    public static ExceptionJournalEntry Create(
        IDateTimeProvider dateTimeProvider,
        string exceptionType,
        string message,
        string stackTrace,
        string requestPath,
        string httpMethod,
        string queryParameters = null,
        string bodyParameters = null)
    {
        if (string.IsNullOrWhiteSpace(exceptionType))
            throw new ArgumentException("Exception type cannot be empty", nameof(exceptionType));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty", nameof(message));
        
        var currentUtcTime = dateTimeProvider.UtcNow;
        var eventId = currentUtcTime.Ticks;
        
        return new ExceptionJournalEntry
        {
            EventId = eventId,
            CreatedAt = currentUtcTime,
            ExceptionType = exceptionType,
            Message = message,
            StackTrace = stackTrace ?? string.Empty,
            RequestPath = requestPath ?? string.Empty,
            HttpMethod = httpMethod ?? string.Empty,
            QueryParameters = queryParameters,
            BodyParameters = bodyParameters
        };
    }
}