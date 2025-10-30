namespace api.Application.DTO;

public record MJournal(string Text, long Id, long EventId, DateTimeOffset CreatedAt);