namespace api.Domain.Repositories;

/// <summary>
/// Repository interface for Node entity operations.
/// </summary>
public interface INodeRepository
{
    Task<Node> GetByIdAsync(long nodeId, CancellationToken cancellationToken = default);
    Task<List<Node>> GetTreeByNameAsync(string treeName, CancellationToken cancellationToken = default);
    Task<bool> HasSiblingWithNameAsync(string treeName, long? parentNodeId, string nodeName, long? excludeNodeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Node node, CancellationToken cancellationToken = default);
    Task UpdateAsync(Node node, CancellationToken cancellationToken = default);
    Task DeleteTreeAsync(long nodeId, CancellationToken cancellationToken = default);
}