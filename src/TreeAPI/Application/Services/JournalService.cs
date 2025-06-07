using Microsoft.EntityFrameworkCore;
using TreeAPI.Application.Abstractions;
using TreeAPI.Application.Services.Common;
using TreeAPI.Domain.Entities;
using TreeAPI.Infrastructure.Persistence;

namespace TreeAPI.Application.Services
{
    public class JournalService : IJournalService
    {
        private readonly TreeApiDbContext _context;

        public JournalService(TreeApiDbContext context)
        {
            _context = context;
        }

        public async Task<MRange_MJournalInfoResponse> GetRangeAsync(int skip, int take, DateTime? from, DateTime? to, string search, CancellationToken cancellationToken)
        {
            if (skip < 0 || take < 0)
            {
                throw new ArgumentException($"Skip or take cannot be negative.");
            }

            var query = _context.JournalEntries.AsQueryable();
            if (from.HasValue)
            {
                query = query.Where(x => x.Timestamp >= from);
            }
            if (to.HasValue)
            {
                query = query.Where(x => x.Timestamp <= to);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.Message.Contains(search));
            }

            var totalCount = query.Count();

            var journalRecords = await query
                .AsNoTracking()
                .OrderByDescending(x => x.Timestamp)
                .Skip(skip)
                .Take(take)
                .Select(x => new MJournalInfoResponse
                {
                    Id = x.Id,
                    EventId = x.EventId,
                    CreatedAt = x.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")
                })
                .ToListAsync();

            var result = new MRange_MJournalInfoResponse
            {
                Skip = skip,
                Count = journalRecords.Count,
                Items = journalRecords
            };
            return result;
        }

        public async Task<MJournalInfoResponse?> GetSingleAsync(long eventId, CancellationToken cancellationToken)
        {
            var journalEntry = await _context.JournalEntries.FirstOrDefaultAsync(x => x.EventId == eventId, cancellationToken);
            if (journalEntry == null)
                return null;

            var result = new MJournalInfoResponse
            {
                Id = journalEntry.Id,
                EventId = journalEntry.EventId,
                CreatedAt = journalEntry.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")
            };
            return result;
        }

        public async Task LogExceptionAsync(long eventId, string? queryParams, string? bodyParams, Exception ex, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var journalEntry = new JournalEntry
            {
                EventId = eventId,
                Timestamp = DateTime.UtcNow,
                Message = ex.Message ?? "Unknown error",
                QueryParameters = queryParams,
                BodyParameters = bodyParams,
                StackTrace = ex.ToString()
            };

            _context.JournalEntries.Add(journalEntry);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
