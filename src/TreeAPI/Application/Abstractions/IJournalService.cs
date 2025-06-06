namespace TreeAPI.Application.Abstractions
{
    public interface IJournalService
    {
        Task LogExceptionAsync(long eventId, string? queryParams, string? bodyParams, Exception ex, CancellationToken cancellationToken);
    }
}
