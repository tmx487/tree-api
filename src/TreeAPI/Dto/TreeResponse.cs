namespace TreeAPI.Dto
{
    public class TreeResponse
    {
        public string TreeName { get; set; } = string.Empty;
        public List<MNode> Nodes { get; set; } = new();
    }
}
