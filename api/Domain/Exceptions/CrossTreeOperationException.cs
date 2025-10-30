namespace api.Domain.Exceptions;

/// <summary>
/// Thrown when trying to move a node to a different tree.
/// </summary>
public class CrossTreeOperationException : SecureException
{
    public CrossTreeOperationException()
        : base("Cannot move node to a different tree")
    {
    }
}