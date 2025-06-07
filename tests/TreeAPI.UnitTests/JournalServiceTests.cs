using Microsoft.EntityFrameworkCore;
using TreeAPI.Application.Services;
using TreeAPI.Application.Services.Common;
using TreeAPI.Domain.Entities;
using TreeAPI.Infrastructure.Persistence;

namespace TreeAPI.UnitTests
{
    public class JournalServiceTests
    {
        private readonly TreeApiDbContext _context;
        private readonly JournalService _journalService;

        public JournalServiceTests()
        {
            var options = new DbContextOptionsBuilder<TreeApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TreeApiDbContext(options);
            _journalService = new JournalService(_context);
        }

        [Fact]
        public async Task GetRange_Should_ReturnListJournalEntries()
        {
            var testData = new List<JournalEntry>();
            testData.AddRange([
                new JournalEntry { Id = 1, EventId = 100, Message = "Test message #1", Timestamp = DateTime.UtcNow.AddDays(-2) },
                    new JournalEntry { Id = 2, EventId = 101, Message = "Another message #2", Timestamp = DateTime.UtcNow.AddDays(-1) },
                    new JournalEntry { Id = 3, EventId = 102, Message = "Third message #3", Timestamp = DateTime.UtcNow },
                    new JournalEntry { Id = 4, EventId = 103, Message = "No hash message", Timestamp = DateTime.UtcNow.AddHours(1) }
                ]);
            _context.JournalEntries.AddRange(testData);
            await _context.SaveChangesAsync();

            int skip = 1, take = 2;
            string search = "#";

            var result = await _journalService.GetRangeAsync(skip, take, null, null, search, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(skip, result.Skip);
            Assert.Equal(take, result.Count);
            Assert.All(result.Items, item =>
                Assert.Contains("#", _getOriginalMessage(item.Id, testData))
           );
        }

        [Fact]
        public async Task GetSingle_Should_ReturnJournalEntry_WhenEnrtyFound()
        {
            var testData = new List<JournalEntry>();
            testData.AddRange([
                new JournalEntry { Id = 1, EventId = 100, Message = "Test message #1", Timestamp = DateTime.UtcNow.AddDays(-2) },
                    new JournalEntry { Id = 2, EventId = 101, Message = "Another message #2", Timestamp = DateTime.UtcNow.AddDays(-1) },
                    new JournalEntry { Id = 3, EventId = 102, Message = "Third message #3", Timestamp = DateTime.UtcNow },
                    new JournalEntry { Id = 4, EventId = 103, Message = "No hash message", Timestamp = DateTime.UtcNow.AddHours(1) }
                ]);
            _context.JournalEntries.AddRange(testData);
            await _context.SaveChangesAsync();

            var eventId = 101;

            var result = await _journalService.GetSingleAsync(eventId, CancellationToken.None);

            Assert.NotNull(result);
            var returnedEntry = await _context.JournalEntries
                .AsNoTracking()
                .Where(x => x.EventId == eventId)
                .Select(x => new MJournalInfoResponse
                {
                    Id = x.Id,
                    EventId = x.EventId,
                    CreatedAt = x.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")
                })
                .FirstAsync();
            Assert.NotNull(returnedEntry);
            Assert.Equal(result.Id, returnedEntry.Id);
            Assert.Equal(result.EventId, returnedEntry.EventId);
            Assert.Equal(result.CreatedAt, returnedEntry.CreatedAt);
        }

        [Fact]
        public async Task GetSingle_Should_ReturnNull_WhenEnrtyNotFound()
        {
            var result = await _journalService.GetSingleAsync(1209, CancellationToken.None);

            Assert.Null(result);
        }

        private string _getOriginalMessage(long id, List<JournalEntry> testData)
            => testData.First(x => x.Id == id).Message;
    }
}
