namespace api.Domain.Abstractions;

public interface IAuthCodeGenerator
{
    string GenerateCode(int length = 32);
}