namespace api.Domain.Exceptions;

/// <summary>
/// Thrown when trying to access a tree that doesn't belong to the user.
/// </summary>
public class TreeAccessDeniedException : SecureException
{
    public TreeAccessDeniedException(string treeName)
        : base($"Access denied to tree '{treeName}'")
    {
    }
}