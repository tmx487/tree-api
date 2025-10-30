using api.Domain;
using api.Domain.Entities;
using api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Repositories;

public class PartnerSecretRepository : IPartnerSecretRepository
{
    private readonly PsqlDbContext _context;

    public PartnerSecretRepository(PsqlDbContext context)
    {
        _context = context;
    }

    public async Task<long> AddAsync(PartnerSecret code, CancellationToken cancellationToken = default)
    {
        await _context.SecretCodes.AddAsync(code, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return code.Id;
    }

    public async Task<PartnerSecret?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentNullException(nameof(code), "Secret code cannot be null or empty.");
        }
        
        var authCode = await _context.SecretCodes
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.SecretCode == code, cancellationToken);
        
        return authCode;
    }
}