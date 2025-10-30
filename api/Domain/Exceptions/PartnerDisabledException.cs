namespace api.Domain.Exceptions;

public class PartnerDisabledException : SecureException
{
    public PartnerDisabledException(long partnerId) 
        : base($"Partner with ID {partnerId} has been disabled.")
    {
    }
}