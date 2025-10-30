using api.Domain.Entities;

namespace api.Domain.Repositories;

public interface IPartnerSecretRepository
{
    Task<long> AddAsync(PartnerSecret code, CancellationToken cancellationToken = default);
    Task<PartnerSecret?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}