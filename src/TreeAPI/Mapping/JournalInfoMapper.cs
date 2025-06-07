using TreeAPI.Application.Services.Common;
using TreeAPI.Dto;

namespace TreeAPI.Mapping
{
    public static class JournalInfoMapper
    {
        public static MRange_MJournalInfo MapToMRange_MJournalInfo(MRange_MJournalInfoResponse reponse)
            => new MRange_MJournalInfo {
                Skip = reponse.Skip,
                Count = reponse.Count,
                Items = _mapItems(reponse.Items).ToList()
            };

        public static MJournalInfo MapToMJournalInfo(MJournalInfoResponse response)
            => new MJournalInfo {
                Id = response.Id,
                EventId = response.EventId,
                CreatedAt = response.CreatedAt,
            };

        private static IEnumerable<MJournalInfo> _mapItems(IEnumerable<MJournalInfoResponse> response)
        {
            var records = new List<MJournalInfo>();
            foreach (var item in response)
            {
                records.Add(new MJournalInfo
                {
                    Id = item.Id,
                    EventId = item.EventId,
                    CreatedAt = item.CreatedAt,
                });
            }
            return records;
        }
    }
}
