using System.ComponentModel.DataAnnotations;

namespace api.Presentation.DTO;

public record AddPartnerRequest([Required] string Name, [Required] bool IsAdmin);