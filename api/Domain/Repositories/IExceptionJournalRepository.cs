namespace api.Domain.Repositories;

/// <summary>
/// Repository interface for ExceptionJournal entity operations.
/// </summary>
public interface IExceptionJournalRepository
{
    Task<ExceptionJournalEntry> GetByEventIdAsync(long eventId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<ExceptionJournalEntry> Items, int TotalCount)> GetRangeAsync(
        int skip, 
        int take, 
        DateTimeOffset? from = null, 
        DateTimeOffset? to = null, 
        string search = null, 
        CancellationToken cancellationToken = default);
    Task AddAsync(ExceptionJournalEntry entry, CancellationToken cancellationToken = default);
}