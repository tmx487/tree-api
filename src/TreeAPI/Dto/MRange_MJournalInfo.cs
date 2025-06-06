namespace TreeAPI.Dto
{
    public class MRange_MJournalInfo
    {
        public int Skip { get; set; }
        public int Count { get; set; }
        public List<MJournalInfo> Items { get; set; } = new();
    }
}
