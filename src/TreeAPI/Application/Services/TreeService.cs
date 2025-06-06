using Microsoft.EntityFrameworkCore;
using TreeAPI.Application.Abstractions;
using TreeAPI.Domain.Entities;
using TreeAPI.Domain.Exceptions;
using TreeAPI.Infrastructure.Persistence;

namespace TreeAPI.Application.Services
{
    public class TreeService : ITreeService
    {
        private readonly TreeApiDbContext _context;

        public TreeService(TreeApiDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task CreateNodeAsync(string treeName, long parentNodeId, string nodeName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(treeName))
                throw new ArgumentException("Tree name cannot be null or empty", nameof(treeName));

            if (string.IsNullOrWhiteSpace(nodeName))
                throw new ArgumentException("Node name cannot be null or empty", nameof(nodeName));

            var parentNode = await _context.Nodes
                .Include(n => n.Tree)
                .FirstOrDefaultAsync(n => n.Id == parentNodeId && n.Tree.TreeName == treeName, cancellationToken);

            if (parentNode == null)
            {
                var treeExists = await _context.Trees
                    .AnyAsync(t => t.TreeName == treeName, cancellationToken);

                if (!treeExists)
                {
                    throw new TreeNotFoundException($"Tree with name '{treeName}' not found.");
                }

                throw new NodeNotFoundException($"Parent node with id {parentNodeId} not found in tree '{treeName}'.");
            }

            var nodeExists = await _context.Nodes
                .AnyAsync(n => n.Name == nodeName && n.TreeId == parentNode.TreeId, cancellationToken);

            if (nodeExists)
            {
                throw new NodeAlreadyExists($"Node with name '{nodeName}' already exists in tree '{treeName}'.");
            }

            var newNode = Node.Create(parentNode.TreeId, parentNodeId, nodeName);
            await _context.AddAsync(newNode, cancellationToken);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new InternalServerErrorException("Error saving new node to the database.", ex);
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("An unexpected error occurred while creating the node.", ex);
            }
        }

        public async Task CreateTreeAsync(string treeName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(treeName))
            {
                throw new ArgumentException("Tree name cannot be null or empty ", nameof(treeName));
            }

            var newTree = new Tree { TreeName = treeName };
            _context.Trees.Add(newTree);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteNodeAsync(string treeName, long nodeId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(treeName))
            {
                throw new ArgumentException("Tree name cannot be null or empty ", nameof(treeName));
            }

            var nodeToDelete = await _context.Nodes
                .Include(n => n.Children)
                .FirstOrDefaultAsync(n => n.Id == nodeId && n.Tree.TreeName == treeName, cancellationToken);

            if (nodeToDelete is null)
            {
                throw new SecureException($"Node with ID {nodeId} not found or does not belong to tree '{treeName}'.");
            }

            if (nodeToDelete.Children.Any())
            {
                throw new NodeHasChildrenException("Delete all children first.");
            }

            _context.Nodes.Remove(nodeToDelete);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves a hierarchical tree of rootNodes by the specified tree name using a recursive CTE.
        /// </summary>
        /// <param name="treeName">The name of the tree to retrieve rootNodes for.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>
        /// A list of root rootNodes, each with their descendants represented via their ParentNodeId relationships.
        /// </returns>
        public async Task<List<Node>> GetTreeNodesAsync(string treeName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sql = $@"
                WITH RECURSIVE TreeHierarchy AS (
                    SELECT
                        n.""Id"",
                        n.""Name"",
                        n.""TreeId"",
                        t.""TreeName"",
                        n.""ParentNodeId""
                    FROM
                        ""Nodes"" n
                    INNER JOIN
                        ""Trees"" t ON n.""TreeId"" = t.""Id""
                    WHERE
                        t.""TreeName"" = {{0}} AND n.""ParentNodeId"" IS NULL
                    UNION ALL
                    SELECT
                        n.""Id"",
                        n.""Name"",
                        n.""TreeId"",
                        t.""TreeName"",
                        n.""ParentNodeId""
                    FROM
                        ""Nodes"" n
                    INNER JOIN
                        TreeHierarchy th ON n.""ParentNodeId"" = th.""Id""
                    INNER JOIN
                        ""Trees"" t ON n.""TreeId"" = t.""Id""
                    WHERE
                        t.""TreeName"" = {{0}}
                )
                SELECT *
                FROM TreeHierarchy;";

            var treeNodes = await _context.Nodes
                .FromSqlRaw(sql, treeName)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var nodes = treeNodes.ToDictionary(n => n.Id);

            foreach (var node in treeNodes)
            {
                if (node.ParentNodeId != null && nodes.TryGetValue(node.ParentNodeId.Value, out var parent))
                {
                    if (parent.Children != null)
                    {
                        parent.AddNewNode(node);
                    }
                }
            }

            var rootNodes = treeNodes.Where(n => n.ParentNodeId == null).ToList();

            return rootNodes;
        }

        public async Task<Tree?> GetTreeByNameAsycn(string treeName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _context.Trees
            .FirstOrDefaultAsync(t => t.TreeName == treeName, cancellationToken);
        }

        public async Task RenameNodeAsync(string treeName, long nodeId, string newNodeName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(treeName))
            {
                throw new ArgumentException("Tree name cannot be null or empty ", nameof(treeName));
            }

            if (string.IsNullOrWhiteSpace(newNodeName))
            {
                throw new ArgumentException("New node's name cannot be null or empty ", nameof(newNodeName));
            }

            var nodeToRename = await _context.Nodes
                .FirstOrDefaultAsync(n => n.Id == nodeId && n.Tree.TreeName == treeName, cancellationToken);

            if (nodeToRename is null)
            {
                throw new SecureException($"Node with ID {nodeId} not found or does not belong to tree {treeName}.");
            }

            if (nodeToRename.Name == newNodeName)
            {
                return;
            }

            var isNodeWithNewNameExist = await _context.Nodes
                .AnyAsync(n => n.TreeId == nodeToRename.TreeId &&
                               n.Name == newNodeName &&
                               n.Id != nodeToRename.Id,
                               cancellationToken);
            if (isNodeWithNewNameExist)
            {
                throw new NodeAlreadyExists($"Node with name {newNodeName} already exists.");
            }

            nodeToRename.RenameNode(newNodeName);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
