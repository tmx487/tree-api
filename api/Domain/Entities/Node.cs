namespace api.Domain;

/// <summary>
/// Represents a node in an independent tree.
/// </summary>
public class Node
{
    public long Id { get; private set; } 
    public string Name { get; private set; }
    public string TreeName { get; private set; } 
    public long? ParentNodeId { get; private set; }
    public Node Parent { get; private set; }
    public ICollection<Node> Children { get; private set; } = new List<Node>();
 
    private Node() { } 

    public static Node CreateRoot(string treeName)
    {
        if (string.IsNullOrWhiteSpace(treeName))
            throw new ArgumentException("Tree name cannot be empty", nameof(treeName));
        
        return new Node
        {
            Name = treeName,
            TreeName = treeName,
            ParentNodeId = null 
        };
    }
    
    public static Node CreateChild(string name, string treeName, long parentNodeId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Node name cannot be empty", nameof(name));
            
        if (string.IsNullOrWhiteSpace(treeName))
            throw new ArgumentException("Tree name cannot be empty", nameof(treeName));

        return new Node
        {
            Name = name,
            TreeName = treeName,
            ParentNodeId = parentNodeId
        };
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Node name cannot be empty", nameof(newName));
        
        Name = newName;
    }
    
    public bool IsRoot() => !ParentNodeId.HasValue;
    
    public void AddChild(Node child) => Children.Add(child);
}