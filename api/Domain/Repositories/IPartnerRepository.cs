using api.Domain.Entities;

namespace api.Domain.Repositories;

public interface IPartnerRepository
{
    Task<long> AddAsync(Partner partner, CancellationToken cancellationToken = default);
    Task<Partner?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}