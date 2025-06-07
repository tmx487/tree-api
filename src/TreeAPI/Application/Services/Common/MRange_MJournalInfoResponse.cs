using TreeAPI.Dto;

namespace TreeAPI.Application.Services.Common
{
    public class MRange_MJournalInfoResponse
    {
        public int Skip { get; set; }
        public int Count { get; set; }
        public List<MJournalInfoResponse> Items { get; set; } = new();
    }
}
