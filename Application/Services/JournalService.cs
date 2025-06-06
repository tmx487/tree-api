using TreeAPI.Application.Abstractions;
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
