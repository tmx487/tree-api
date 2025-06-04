namespace TreeAPI.Dto
{
    public class MJournal
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
