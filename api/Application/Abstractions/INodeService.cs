using api.Application.DTO;

namespace api.Application.Abstractions;

public interface INodeService
{
    Task<NodeDto> GetTreeAsync(string treeName, CancellationToken cancellationToken = default);
    Task CreateNodeAsync(string treeName, long? parentNodeId, string nodeName, CancellationToken cancellationToken = default);
    Task RenameNodeAsync(long nodeId, string newNodeName, CancellationToken cancellationToken = default);
    Task DeleteNodeAsync(long nodeId, CancellationToken cancellationToken = default);
}