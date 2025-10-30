using api.Domain.Entities;
using api.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Repositories;

public class PartnerRepository :  IPartnerRepository
{
    private readonly PsqlDbContext _context;

    public PartnerRepository(PsqlDbContext context)
    {
        _context = context;
    }
    
    public async Task<long> AddAsync(Partner partner, CancellationToken cancellationToken = default)
    {
        await _context.Partners.AddAsync(partner, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return partner.Id;
    }

    public async Task<Partner?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Partners
            .AsNoTracking() 
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}