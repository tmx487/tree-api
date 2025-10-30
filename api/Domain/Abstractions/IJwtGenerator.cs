using System.Security.Claims;
using api.Presentation.DTO;

namespace api.Domain.Abstractions;

public interface IJwtGenerator
{
    TokenInfo GenerateToken(IEnumerable<Claim> claims);
}