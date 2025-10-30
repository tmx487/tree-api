namespace api.Domain.Exceptions;

/// <summary>
/// Base class for secure exceptions that should be logged with full details
/// and returned to the client with structured error information.
/// </summary>
public class SecureException : Exception
{
    public SecureException(string message) : base(message)
    {
    }

    public SecureException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}