using System.Security.Cryptography;
using api.Domain.Abstractions;

namespace api.Infrastructure.Services;

public class AuthCodeGenerator : IAuthCodeGenerator
{
    public string GenerateCode(int length = 32)
    {
        var randomNumberBytes = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumberBytes);
        }

        string base64Code = Convert.ToBase64String(randomNumberBytes);

        return base64Code
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}