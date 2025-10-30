namespace api.Domain.Exceptions;

public class PartnerSecretNotFoundException : SecureException
{
    public PartnerSecretNotFoundException(string code)
        : base($"AuthCode {code} was not found.")
    {
    }
}