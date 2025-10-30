using api.Application.DTO;
using api.Presentation.DTO;

namespace api.Application.Abstractions;

public interface IPartnerService
{
    Task<TokenInfo> GetAuthTokenAsync(string code, CancellationToken cancellationToken = default);
    Task<PartnerCreationResult> AddAsync(string name, bool isAdmin, CancellationToken cancellationToken = default);
}