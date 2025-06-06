using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeAPI.Domain.Entities
{
    public class Node
    {
        private List<Node> _children = new List<Node>();
        private Node() { }
        private Node(long parentNodeId, string nodeName)
        {
            ParentNodeId = parentNodeId;
            Name = nodeName;
        }

        [Key]
        public long Id { get; init; }

        [Required]
        public string Name { get; private set; } = default!;
        public long TreeId { get; init; }

        [ForeignKey("TreeId")]
        public Tree Tree { get; init; }
        public long? ParentNodeId { get; init; }

        [ForeignKey("ParentNodeId")]
        public Node? Parent { get; init; }

        public IReadOnlyCollection<Node> Children => _children;

        public static Node Create(long parentNodeId, string nodeName)
            => new Node(parentNodeId,nodeName);

        public void AddNewNode(Node node)
        {
            _children.Add(node);
        }

        public void RenameNode(string newNodeName)
        {
            Name = newNodeName;
        }
    }
}
