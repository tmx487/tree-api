namespace api.Domain.Exceptions;

public class JournalEntryNotFoundException : SecureException
{
    public JournalEntryNotFoundException(long eventId)
        : base($"Journal entry with event ID {eventId} was not found")
    {
    }
}