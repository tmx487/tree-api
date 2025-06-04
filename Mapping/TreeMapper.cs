using TreeAPI.Domain.Entities;
using TreeAPI.Dto;

namespace TreeAPI.Mapping
{
    public static class TreeMapper
    {
        public static MNode MapToMNode(Node node, Dictionary<long, MNode> mNodeDictionary)
        {
            if (mNodeDictionary.TryGetValue(node.Id, out var mNode))
            {
                return mNode;
            }

            var newMNode = new MNode
            {
                Id = node.Id,
                Name = node.Name,
                Children = new List<MNode>()
            };
            mNodeDictionary[node.Id] = newMNode;

            foreach (var child in node.Children)
            {
                newMNode.Children.Add(MapToMNode(child, mNodeDictionary));
            }

            return newMNode;
        }

        public static TreeResponse MapToTreeResponse(Tree tree, List<Node> rootNodes)
        {
            var resultMNodes = new List<MNode>();
            var mNodeDictionary = new Dictionary<long, MNode>();

            foreach (var rootNode in rootNodes)
            {
                resultMNodes.Add(MapToMNode(rootNode, mNodeDictionary));
            }

            return new TreeResponse
            {
                TreeName = tree.TreeName,
                Nodes = resultMNodes
            };
        }
    }
}
