using System.ComponentModel.DataAnnotations;

namespace TreeAPI.Domain.Entities
{
    public class JournalEntry
    {
        [Key]
        public long Id { get; set; }
        public long EventId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Message { get; set; } = default!;
        public string? StackTrace { get; set; }
        public string? QueryParameters { get; set; }
        public string? BodyParameters { get; set; }
    }
}
