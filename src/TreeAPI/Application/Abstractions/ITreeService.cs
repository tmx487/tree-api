using TreeAPI.Domain.Entities;

namespace TreeAPI.Application.Abstractions
{
    public interface ITreeService
    {
        Task CreateNodeAsync(string treeName, long parentNodeId, string nodeName, CancellationToken cancellationToken);
        Task CreateTreeAsync(string treeName, CancellationToken cancellationToken);
        Task DeleteNodeAsync(string treeName, long nodeId, CancellationToken cancellationToken);
        Task RenameNodeAsync(string treeName, long nodeId, string newNodeName, CancellationToken cancellationToken);
        Task<List<Node>> GetTreeNodesAsync(string treeName, CancellationToken cancellationToken);
        Task<Tree?> GetTreeByNameAsycn(string treeName, CancellationToken cancellationToken);
    }
}
