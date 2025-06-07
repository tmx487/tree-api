namespace TreeAPI.Dto
{
    public class VJournalFilter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Search { get; set; } = string.Empty;
    }
}
