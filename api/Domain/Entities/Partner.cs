using api.Domain.Abstractions;

namespace api.Domain.Entities;

/// <summary>
/// Represents a user in the system (optional, for authentication).
/// </summary>
public class Partner
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public PartnerRole Role { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    private Partner()
    {
    }

    public static Partner Create(
        IDateTimeProvider dateTimeProvider,
        string name,
        bool isAdmin)
    { 
        return new Partner
        {
            Name = name,
            Role = isAdmin ? PartnerRole.Admin : PartnerRole.Client,
            CreatedAt = dateTimeProvider.UtcNow
        };
    }
}