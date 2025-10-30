namespace api.Domain.Exceptions;

/// <summary>
/// Thrown when trying to delete a node that has children.
/// </summary>
public class NodeHasChildrenException : SecureException
{
    public NodeHasChildrenException()
        : base("You have to delete all children nodes first")
    {
    }
}