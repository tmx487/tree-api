using TreeAPI.Application.Services.Common;
using TreeAPI.Domain.Entities;

namespace TreeAPI.Application.Abstractions
{
    public interface IJournalService
    {
        Task<MRange_MJournalInfoResponse> GetRangeAsync(int skip, int take, DateTime? from, DateTime? to, string search, CancellationToken cancellationToken);
        Task<MJournalInfoResponse?> GetSingleAsync(long eventId, CancellationToken cancellationToken);
        Task LogExceptionAsync(long eventId, string? queryParams, string? bodyParams, Exception ex, CancellationToken cancellationToken);
    }
}
