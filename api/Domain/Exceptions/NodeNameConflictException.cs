namespace api.Domain.Exceptions;

/// <summary>
/// Thrown when a node with the same name already exists among siblings.
/// </summary>
public class NodeNameConflictException : SecureException
{
    public NodeNameConflictException(string nodeName)
        : base($"A node with the name '{nodeName}' already exists among siblings")
    {
    }
}