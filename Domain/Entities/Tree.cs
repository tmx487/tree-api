using System.ComponentModel.DataAnnotations;

namespace TreeAPI.Domain.Entities
{
    public class Tree
    {
        private List<Node> _nodes = new List<Node>();
        [Key]
        public long Id { get; init; }
        public string TreeName { get; init; } = default!;
        public IReadOnlyCollection<Node> Nodes => _nodes;
    }
}
