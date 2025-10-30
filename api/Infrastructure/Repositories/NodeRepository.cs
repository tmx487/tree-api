using api.Domain;
using api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Repositories;

public class NodeRepository : INodeRepository
{
    private readonly PsqlDbContext _context;

    public NodeRepository(PsqlDbContext context)
    {
        _context = context;
    }

    public Task<Node> GetByIdAsync(long nodeId, CancellationToken cancellationToken = default)
    {
        return _context.Nodes
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == nodeId, cancellationToken);
    }

    public async Task<List<Node>> GetTreeByNameAsync(string treeName, CancellationToken cancellationToken = default)
    {
        var sql = $@"
        WITH RECURSIVE TreeHierarchy AS (
            SELECT 
                n.* FROM 
                ""Nodes"" n
            WHERE 
                n.""TreeName"" = {{0}} AND n.""ParentNodeId"" IS NULL

            UNION ALL

            SELECT 
                n.*
            FROM 
                ""Nodes"" n
            INNER JOIN 
                TreeHierarchy th ON n.""ParentNodeId"" = th.""Id""
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
                    parent.AddChild(node);
                }
            }
        }

        var rootNodes = treeNodes.Where(n => n.ParentNodeId == null).ToList();

        return rootNodes;
    }

    public Task<bool> HasSiblingWithNameAsync(string treeName, long? parentNodeId, string nodeName,
        long? excludeNodeId = null,
        CancellationToken cancellationToken = default)
    {
        return _context.Nodes.AnyAsync(n =>
                n.TreeName == treeName &&
                n.ParentNodeId == parentNodeId &&
                n.Name == nodeName &&
                n.Id != excludeNodeId,
            cancellationToken);
    }

    public async Task AddAsync(Node node, CancellationToken cancellationToken = default)
    {
        await _context.Nodes.AddAsync(node, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(Node node, CancellationToken cancellationToken = default)
    {
        _context.Nodes.Update(node);
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTreeAsync(long nodeId, CancellationToken cancellationToken = default)
    {
        var nodeToDelete = await _context.Nodes
            .AsNoTracking()
            .Select(n => new { n.Id, n.TreeName })
            .FirstOrDefaultAsync(n => n.Id == nodeId, cancellationToken);

        if (nodeToDelete == null) return;

        string sql = $@"
                WITH RECURSIVE Descendants AS (
                    SELECT ""Id""
                    FROM ""Nodes""
                    WHERE ""Id"" = {{0}}
                    
                    UNION ALL
                    
                    SELECT n.""Id""
                    FROM ""Nodes"" n
                    INNER JOIN Descendants d ON n.""ParentNodeId"" = d.""Id""
                )
                DELETE FROM ""Nodes""
                WHERE ""Id"" IN (SELECT ""Id"" FROM Descendants);";

        await _context.Database.ExecuteSqlRawAsync(sql, [nodeId], cancellationToken);
    }
}