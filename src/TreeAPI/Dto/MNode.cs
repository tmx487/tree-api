namespace TreeAPI.Dto
{
    public class MNode
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public List<MNode> Children { get; set; } = new();
    }

}
