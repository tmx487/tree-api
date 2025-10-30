namespace api.Domain.Exceptions;

/// <summary>
/// Thrown when trying to perform an operation on a non-existent node.
/// </summary>
public class NodeNotFoundException : SecureException
{
    public NodeNotFoundException(long nodeId)
        : base($"Node with ID {nodeId} was not found")
    {
    }
}