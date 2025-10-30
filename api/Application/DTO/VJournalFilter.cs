namespace api.Application.DTO;

public record VJournalFilter(DateTimeOffset? From, DateTimeOffset? To, string? Search);