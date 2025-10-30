namespace api.Application.DTO;

public class NodeDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public List<NodeDto> Children { get; set; } = new List<NodeDto>();
}