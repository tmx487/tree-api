using api.Domain;
using api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Repositories;

public class ExceptionJournalRepository : IExceptionJournalRepository
{
    private readonly PsqlDbContext _context;

    public ExceptionJournalRepository(PsqlDbContext context)
    {
        _context = context;
    }

    public async Task<ExceptionJournalEntry> GetByEventIdAsync(long eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Journal
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken);
    }

    public async Task<(IEnumerable<ExceptionJournalEntry> Items, int TotalCount)> GetRangeAsync(
        int skip, int take, DateTimeOffset? from = null, DateTimeOffset? to = null, string search = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ExceptionJournalEntry> query = _context.Journal.AsNoTracking();

        if (from.HasValue)
        {
            query = query.Where(e => e.CreatedAt >= from.Value);
        }
            
        if (to.HasValue)
        {
            query = query.Where(e => e.CreatedAt <= to.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            string lowerSearch = search.ToLower();
            query = query.Where(e =>
                EF.Functions.Like(e.ExceptionType.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(e.Message.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(e.RequestPath.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(e.StackTrace.ToLower(), $"%{lowerSearch}%")
            );
        }

        int totalCount = await query.CountAsync(cancellationToken);

        query = query.OrderByDescending(e => e.CreatedAt);

        List<ExceptionJournalEntry> items = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(ExceptionJournalEntry entry, CancellationToken cancellationToken = default)
    {
        await _context.Journal.AddAsync(entry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}