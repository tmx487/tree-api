namespace api.Presentation.DTO;

public record ErrorResponse(string Type, string Id, Dictionary<string, string> Data);