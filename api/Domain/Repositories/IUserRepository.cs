using api.Domain.Entities;

namespace api.Domain.Repositories;

/// <summary>
/// Repository interface for User entity operations.
/// </summary>
public interface IUserRepository
{
    Task<Partner> GetByIdAsync(long userId, CancellationToken cancellationToken = default);
    Task<Partner> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Partner> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(Partner partner, CancellationToken cancellationToken = default);
    Task UpdateAsync(Partner partner, CancellationToken cancellationToken = default);
}