using api.Application.Abstractions;
using api.Application.DTO;
using api.Domain;
using api.Domain.Exceptions;
using api.Domain.Repositories;

namespace api.Application.Services;

public class NodeService : INodeService
{
    private readonly INodeRepository _nodeRepository;

    public NodeService(INodeRepository nodeRepository)
    {
        _nodeRepository = nodeRepository;
    }

    public async Task<NodeDto> GetTreeAsync(string treeName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(treeName))
            throw new ArgumentException("Tree name cannot be empty", nameof(treeName));

        var allNodes = await _nodeRepository.GetTreeByNameAsync(treeName, cancellationToken);

        var rootNodes = allNodes.Where(n => n.ParentNodeId == null).ToList();

        if (allNodes.Count == 0)
        {
            var newNode = Node.CreateRoot(treeName); 
            await _nodeRepository.AddAsync(newNode, cancellationToken);
        
            return MapToDto(newNode);
        }
    
        if (rootNodes.Count > 1)
        {
            throw new InvalidOperationException($"Multiple root nodes found for tree '{treeName}'.");
        }
    
        var rootNode = rootNodes.Single();

        return MapToDto(rootNode);
    }

    public async Task CreateNodeAsync(
        string treeName,
        long? parentNodeId,
        string nodeName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(treeName))
            throw new ArgumentException("Tree name cannot be empty", nameof(treeName));

        if (string.IsNullOrWhiteSpace(nodeName))
            throw new ArgumentException("Node name cannot be empty", nameof(nodeName));
        
        var rootDto = await GetTreeAsync(treeName, cancellationToken);
        long targetParentId = parentNodeId ?? rootDto.Id;

        await CreateChildNodeAsync(treeName, targetParentId, nodeName, cancellationToken);
    }

    public async Task RenameNodeAsync(
        long nodeId,
        string newNodeName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newNodeName))
            throw new ArgumentException("Node name cannot be empty", nameof(newNodeName));

        var node = await _nodeRepository.GetByIdAsync(nodeId, cancellationToken);

        if (node == null)
        {
            throw new NodeNotFoundException(nodeId);
        }
        
        if (node.IsRoot())
        {
            throw new SecureException("Cannot rename root node");
        }

        if (node.Name == newNodeName)
        {
            return;
        }

        var hasConflict = await _nodeRepository.HasSiblingWithNameAsync(
            node.TreeName,
            node.ParentNodeId,
            newNodeName,
            excludeNodeId: nodeId,
            cancellationToken);

        if (hasConflict)
        {
            throw new NodeNameConflictException(newNodeName);
        }

        node.Rename(newNodeName);
        await _nodeRepository.UpdateAsync(node, cancellationToken);
    }

    public async Task DeleteNodeAsync(long nodeId, CancellationToken cancellationToken = default)
    {
        var node = await _nodeRepository.GetByIdAsync(nodeId, cancellationToken);

        if (node == null)
        {
            throw new NodeNotFoundException(nodeId);
        }

        // if (node.IsRoot())
        // {
        //     throw new SecureException("Cannot delete root node. Delete entire tree instead.");
        // }

        await _nodeRepository.DeleteTreeAsync(nodeId, cancellationToken);
    }
    private async Task CreateChildNodeAsync(
        string treeName,
        long parentNodeId,
        string nodeName,
        CancellationToken cancellationToken)
    {
        var parentNode = await _nodeRepository.GetByIdAsync(parentNodeId, cancellationToken);

        if (parentNode == null)
        {
            throw new NodeNotFoundException(parentNodeId);
        }

        if (parentNode.TreeName != treeName)
        {
            throw new CrossTreeOperationException();
        }

        var hasConflict = await _nodeRepository.HasSiblingWithNameAsync(
            treeName,
            parentNodeId,
            nodeName,
            excludeNodeId: null,
            cancellationToken);

        if (hasConflict)
        {
            throw new NodeNameConflictException(nodeName);
        }

        var newNode = Node.CreateChild(nodeName, treeName, parentNodeId);
        await _nodeRepository.AddAsync(newNode, cancellationToken);
    }

    private NodeDto MapToDto(Node node)
    {
        return new NodeDto
        {
            Id = node.Id,
            Name = node.Name,
            Children = node.Children?.Select(MapToDto).ToList() ?? new List<NodeDto>()
        };
    }
}