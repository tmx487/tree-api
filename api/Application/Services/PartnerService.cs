using System.Security.Claims;
using api.Application.Abstractions;
using api.Application.DTO;
using api.Domain.Abstractions;
using api.Domain.Entities;
using api.Domain.Exceptions;
using api.Domain.Repositories;
using api.Presentation.DTO;

namespace api.Application.Services;

public class PartnerService : IPartnerService
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IPartnerSecretRepository _partnerSecretRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuthCodeGenerator _codeGenerator;

    public PartnerService(
        IJwtGenerator jwtGenerator,
        IPartnerSecretRepository partnerSecretRepository,
        IPartnerRepository partnerRepository, 
        IDateTimeProvider dateTimeProvider, 
        IAuthCodeGenerator codeGenerator)
    {
        _jwtGenerator = jwtGenerator;
        _partnerSecretRepository = partnerSecretRepository;
        _partnerRepository = partnerRepository;
        _dateTimeProvider = dateTimeProvider;
        _codeGenerator = codeGenerator;
    }

    public async Task<TokenInfo> GetAuthTokenAsync(string secretCode, CancellationToken cancellationToken = default)
    {
        var partnerSecret = await _partnerSecretRepository.GetByCodeAsync(secretCode, cancellationToken);
    
        if (partnerSecret == null)
        {
            throw new PartnerSecretNotFoundException(secretCode);
        }
        
        var partner = await _partnerRepository.GetByIdAsync(partnerSecret.PartnerId, cancellationToken);
        if (partner == null)
        {
            throw new PartnerDisabledException(partnerSecret.PartnerId);
        }
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, partner.Id.ToString()), 
            new Claim(ClaimTypes.Role, partner.Role.ToString()) 
        };

        return _jwtGenerator.GenerateToken(claims);
    }

    public async Task<PartnerCreationResult> AddAsync(string name, bool isAdmin, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }
        var newPartner = Partner.Create(_dateTimeProvider, name, isAdmin);
        
        long newPartnerId = await _partnerRepository.AddAsync(newPartner, cancellationToken);
    
        string secretCode = _codeGenerator.GenerateCode(); 
    
        var authCode = PartnerSecret.Create(
            _dateTimeProvider, 
            newPartnerId, 
            secretCode
        );
    
        await _partnerSecretRepository.AddAsync(authCode, cancellationToken);
    
        return new PartnerCreationResult(newPartnerId, secretCode);
    }
}