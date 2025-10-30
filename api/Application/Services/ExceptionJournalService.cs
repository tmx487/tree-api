using api.Application.DTO;
using api.Domain;
using api.Domain.Exceptions;
using api.Domain.Repositories;

namespace api.Application.Services;

public class ExceptionJournalService : IExceptionJournalService
{
    private readonly IExceptionJournalRepository _journalRepository;

    public ExceptionJournalService(IExceptionJournalRepository journalRepository)
    {
        _journalRepository = journalRepository;
    }

    public async Task<MJournal> GetSingleAsync(long eventId, CancellationToken cancellationToken = default)
    {
        var entry = await _journalRepository.GetByEventIdAsync(eventId, cancellationToken);

        if (entry == null)
        {
            throw new JournalEntryNotFoundException(eventId);
        }

        return MapToMJournal(entry);

    }

    public async Task<MRangeJournalInfo> GetRangeAsync(int skip, int take, VJournalFilter filter, CancellationToken cancellationToken = default)
    {
        var (entries, totalCount) = await _journalRepository.GetRangeAsync(
            skip, 
            take, 
            filter?.From, 
            filter?.To, 
            filter?.Search, 
            cancellationToken);

        var items = entries.Select(MapToMJournalInfo).ToList();

        return new MRangeJournalInfo(skip, totalCount, items);
    }
    
    private static MJournalInfo MapToMJournalInfo(ExceptionJournalEntry entry)
    {
        return new MJournalInfo(entry.Id, entry.EventId, entry.CreatedAt);
    }
    
    private static MJournal MapToMJournal(ExceptionJournalEntry entry)
    {
        return new MJournal(entry.Message, entry.Id, entry.EventId, entry.CreatedAt);
    }

}