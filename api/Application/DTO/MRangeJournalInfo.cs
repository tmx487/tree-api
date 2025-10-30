namespace api.Application.DTO;

public record MRangeJournalInfo(int Skip, int Count, List<MJournalInfo> Items);