using Microsoft.EntityFrameworkCore;
using Moq;
using TreeAPI.Application.Services;
using TreeAPI.Domain.Entities;
using TreeAPI.Domain.Exceptions;
using TreeAPI.Infrastructure.Persistence;

namespace TreeAPI.UnitTests;

public class TreeServiceTests
{
    private readonly TreeService _treeService;
    private readonly TreeApiDbContext _context;

    public TreeServiceTests()
    {
        var options = new DbContextOptionsBuilder<TreeApiDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

        _context = new TreeApiDbContext(options);

        _treeService = new TreeService(_context);
    }

    [Fact]
    public async Task CreateNode_Should_SuccessfullySavesNodeIntoDb()
    {
        var tree = new Tree { Id = 109, TreeName = "Tree_43gfow3" };
        await _context.Trees.AddAsync(tree);
        await _context.SaveChangesAsync();

        var parentNode = Node.Create(tree.Id, null, "Node_g934hf");
        await _context.Nodes.AddAsync(parentNode);
        await _context.SaveChangesAsync();

        var newNodeName = "Node_o2fi3o";

        await _treeService.CreateNodeAsync(tree.TreeName, parentNode.Id, newNodeName, CancellationToken.None);

        var createdNode = await _context.Nodes.AsNoTracking().FirstOrDefaultAsync(n => n.Name == newNodeName, default);
        Assert.NotNull(createdNode);
        Assert.Equal(newNodeName, createdNode.Name);
    }

    [Fact]
    public async Task CreateNode_Should_ThrowException_WhenTreeNotFound()
    {
        var tree = new Tree { Id = 109, TreeName = "Tree_43gfow3" };
        await _context.Trees.AddAsync(tree);
        await _context.SaveChangesAsync();

        var parentNode = Node.Create(tree.Id, null, "Node_g934hf");
        await _context.Nodes.AddAsync(parentNode);
        await _context.SaveChangesAsync();

        var newNodeName = "Node_e3gf4hf";
        var nonExistingTreeName = "Tree_Test";

        await Assert.ThrowsAsync<TreeNotFoundException>(
            () => _treeService.CreateNodeAsync(nonExistingTreeName, parentNode.Id, newNodeName, CancellationToken.None)
            );
    }

    [Fact]
    public async Task CreateNode_Should_ThrowException_WhenNodeAlreadyExists()
    {
        var tree = new Tree { Id = 109, TreeName = "Tree_43gfow3" };
        await _context.Trees.AddAsync(tree);
        await _context.SaveChangesAsync();

        var parentNode = Node.Create(tree.Id, null, "Node_g934hf");
        await _context.Nodes.AddAsync(parentNode);
        await _context.SaveChangesAsync();

        await Assert.ThrowsAsync<NodeAlreadyExists>(() => 
            _treeService.CreateNodeAsync(tree.TreeName, parentNode.Id, parentNode.Name, default)
            );
    }

    [Fact]
    public async Task CreateNode_Should_ThrowException_WhenParentNodeBelonsToDifferentTree()
    {
        var tree1 = new Tree { Id = 109, TreeName = "Tree_43gfow3" };
        var tree2 = new Tree { Id = 185, TreeName = "Tree_f253dd2" };
        await _context.Trees.AddRangeAsync([tree1, tree2]);
        await _context.SaveChangesAsync();

        var parentNode = Node.Create(tree1.Id, null, "Node_g934hf");
        await _context.Nodes.AddAsync(parentNode);
        await _context.SaveChangesAsync();

        var newNodeName = "Node_e3gf4hf";

        await Assert.ThrowsAsync<NodeNotFoundException>(() =>
            _treeService.CreateNodeAsync(tree2.TreeName, parentNode.Id, newNodeName, default)
            );
    }

    [Fact]
    public async Task DeleteNode_Should_ReturnSuccess_WhenNodeDeleted()
    {
        var tree = new Tree { Id = 100, TreeName = "Tree_vjo2348r" };
        await _context.Trees.AddAsync(tree);
        await _context.SaveChangesAsync();

        var parentNode = Node.Create(tree.Id, null, "Node_d892fhs");
        await _context.Nodes.AddAsync(parentNode);
        await _context.SaveChangesAsync();

        await _treeService.DeleteNodeAsync(tree.TreeName, parentNode.Id, CancellationToken.None);

        var deletedNode = await _context.Nodes.SingleOrDefaultAsync(n => n.Id == parentNode.Id);
        Assert.Null(deletedNode);
    }

    [Fact]
    public async Task DeleteNode_Should_ThrowException_WhenNodeToDeleteNotFoundOrBelongsToDifferentTree()
    {
        var tree1 = new Tree { Id = 100, TreeName = "Tree_vjo2348r" };
        var tree2 = new Tree { Id = 110, TreeName = "Tree_8437hfew" };
        await _context.Trees.AddRangeAsync([tree1, tree2]);
        await _context.SaveChangesAsync();

        var node = Node.Create(tree1.Id, null, "Node_d892fhs");
        await _context.Nodes.AddAsync(node);
        await _context.SaveChangesAsync();

        await Assert.ThrowsAsync<SecureException>(() =>
        _treeService.DeleteNodeAsync(tree2.TreeName, node.Id, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteNode_Should_ThrowException_WhenNodeHasChildren()
    {
        var tree = new Tree { Id = 100, TreeName = "Tree_vjo2348r" };
        await _context.Trees.AddAsync(tree);
        await _context.SaveChangesAsync();

        var parentNode = Node.Create(tree.Id, null, "Node_d892fhs");
        await _context.Nodes.AddAsync(parentNode);
        await _context.SaveChangesAsync();

        var childNode = Node.Create(tree.Id, parentNode.Id, "Node_0dok2f9s");
        await _context.Nodes.AddAsync(childNode);
        await _context.SaveChangesAsync();

        await Assert.ThrowsAsync<NodeHasChildrenException>(() =>
        _treeService.DeleteNodeAsync(tree.TreeName, parentNode.Id, CancellationToken.None));
    }

    [Fact]
    public async Task RenameNode_Should_ReturnSuccess_WhenNodeRenamed()
    {
        var tree = new Tree { Id = 100, TreeName = "Tree_vjo2348r" };
        await _context.Trees.AddAsync(tree);
        await _context.SaveChangesAsync();

        var node = Node.Create(tree.Id, null, "Node_d892fhs");
        await _context.Nodes.AddAsync(node);
        await _context.SaveChangesAsync();

        var newNodeName = "Node_c092jv";

        await _treeService.RenameNodeAsync(tree.TreeName, node.Id, newNodeName, CancellationToken.None);

        var renamedNode = await _context.Nodes.SingleOrDefaultAsync(n => n.Id == node.Id);
        Assert.NotNull(renamedNode);
        Assert.Equal(newNodeName, renamedNode.Name);
    }

    [Fact]
    public async Task RenameNode_Should_ThrowException_WhenNotFoundORBelonsToDifferentTree()
    {
        var tree1 = new Tree { Id = 100, TreeName = "Tree_vjo2348r" };
        var tree2 = new Tree { Id = 182, TreeName = "Tree_jf89384r" };
        await _context.Trees.AddRangeAsync([tree1, tree2]);
        await _context.SaveChangesAsync();

        var node = Node.Create(tree1.Id, null, "Node_d892fhs");
        await _context.Nodes.AddAsync(node);
        await _context.SaveChangesAsync();

        var newNodeName = "Node_c092jv";

        await Assert.ThrowsAsync<SecureException>(() =>
        _treeService.RenameNodeAsync(tree2.TreeName, node.Id, newNodeName, CancellationToken.None));
    }

    [Fact]
    public async Task RenameNode_Should_ReturnNothing_WhenNameOfNodeNotChanges()
    {
        var tree = new Tree { Id = 100, TreeName = "Tree_vjo2348r" };
        await _context.Trees.AddAsync(tree);
        await _context.SaveChangesAsync();

        var initialNodeName = "Node_d892fhs";
        var node = Node.Create(tree.Id, null, initialNodeName);
        await _context.Nodes.AddAsync(node);
        await _context.SaveChangesAsync();

        var newNodeName = initialNodeName;
        await _treeService.RenameNodeAsync(tree.TreeName, node.Id, newNodeName, CancellationToken.None);

        var nodeInDB = await _context.Nodes.AsNoTracking().SingleOrDefaultAsync(n => n.Id == node.Id);
        Assert.NotNull(nodeInDB);
        Assert.Equal(initialNodeName, nodeInDB.Name);
    }

    [Fact]
    public async Task RenameNode_Should_ThrowException_WhenNodeWithNewNameAlreadyExistsInThisTree()
    {
        var tree = new Tree { Id = 100, TreeName = "Tree_vjo2348r" };
        await _context.Trees.AddAsync(tree);
        await _context.SaveChangesAsync();

        var nodeToRename = Node.Create(tree.Id, null, "Node_d892fhs");
        var conflictingName = "Node_09230ri";
        var conflictingNode = Node.Create(tree.Id, null, conflictingName);
        await _context.Nodes.AddRangeAsync([nodeToRename, conflictingNode]);
        await _context.SaveChangesAsync();

        var newNodeName = conflictingName;

        await Assert.ThrowsAsync<NodeAlreadyExists>(() =>
            _treeService.RenameNodeAsync(tree.TreeName, nodeToRename.Id, newNodeName, CancellationToken.None));

        var nodeInDB = await _context.Nodes.AsNoTracking().SingleOrDefaultAsync(n => n.Id == nodeToRename.Id, CancellationToken.None);
        Assert.NotNull(nodeInDB);
        Assert.Equal(nodeToRename.Name, nodeInDB.Name);
    }
}
