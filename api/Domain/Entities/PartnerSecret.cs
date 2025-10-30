using api.Domain.Abstractions;

namespace api.Domain.Entities;

public class PartnerSecret
{
    public long Id { get; private set; }
    public string SecretCode { get; private set; }
    public long PartnerId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private PartnerSecret() {}

    public static PartnerSecret Create(
        IDateTimeProvider dateTimeProvider, 
        long partnerId, 
        string secretCode)
    {
        if (string.IsNullOrWhiteSpace(secretCode))
            throw new ArgumentException("SecretCode cannot be empty.", nameof(secretCode));
        if (partnerId <= 0)
            throw new ArgumentException("Partner ID must be valid.", nameof(partnerId));

        return new PartnerSecret
        {
            SecretCode = secretCode,
            PartnerId = partnerId,
            CreatedAt = dateTimeProvider.UtcNow
        };
    }
}