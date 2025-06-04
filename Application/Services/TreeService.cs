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

        public async Task CreateNewNodeAsycn(string treeName, long parentNodeId, string nodeName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(treeName))
                throw new ArgumentException("Tree name cannot be null or empty ", nameof(treeName));

            if (string.IsNullOrWhiteSpace(nodeName))
                throw new ArgumentException("Node name cannot be null or empty ", nameof(nodeName));

            var parentNode = await _context.Nodes.FirstOrDefaultAsync(
                n => n.Id == parentNodeId && n.Tree.TreeName.Equals(treeName, StringComparison.OrdinalIgnoreCase),
                cancellationToken) ?? throw new NodeNotFoundException($"Parent node with id {parentNodeId} not found in this isTreeExists.");

            if (!parentNode.Tree.TreeName.Equals(treeName, StringComparison.OrdinalIgnoreCase))
            {
                throw new SecureException($"Parent node with id {parentNodeId} belongs to a different isTreeExists ({parentNode.Tree.TreeName}).");
            }

            if (await _context.Nodes.AnyAsync(
                n => n.ParentNodeId == parentNodeId && n.Tree.TreeName.Equals(treeName, StringComparison.OrdinalIgnoreCase) && n.Name == nodeName,
                cancellationToken))
            {
                throw new SecureException("A sibling node with the same name already exists.");
            }

            var newNode = Node.Create(parentNodeId, nodeName);

            await _context.AddAsync(newNode, cancellationToken);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new InternalServerErrorException("Error saving new node to the database.", ex, ex.StackTrace);
            }
            catch (Exception ex)
            {
                throw new InternalServerErrorException("An unexpected error occurred while creating the node.", ex, ex.StackTrace);
            }
        }

        public async Task CreateTreeAsync(string treeName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(treeName))
            {
                throw new ArgumentException("Tree name cannot be null or empty ", nameof(treeName));
            }

            var newTree = new Tree { TreeName = treeName };
            _context.Trees.Add(newTree);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task DeleteNodeAsync(string treeName, long nodeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
        => await _context.Trees
            .FirstOrDefaultAsync(t => t.TreeName == treeName, cancellationToken);

        public Task RenameNodeAsync(string treeName, long nodeId, string newNodeName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
