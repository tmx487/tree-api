using api.Application.DTO;

namespace api.Application;

public interface IExceptionJournalService
{
    Task<MJournal> GetSingleAsync(long eventId, CancellationToken cancellationToken = default);

    Task<MRangeJournalInfo> GetRangeAsync(
        int skip, 
        int take, 
        VJournalFilter filter, 
        CancellationToken cancellationToken = default);
}