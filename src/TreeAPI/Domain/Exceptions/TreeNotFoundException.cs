
namespace TreeAPI.Domain.Exceptions
{
    public class TreeNotFoundException : SecureException
    {
        public TreeNotFoundException(string message, string? callStack = null, string? queryParameters = null, string? bodyParameters = null) : base(message, callStack, queryParameters, bodyParameters)
        {
        }

        public TreeNotFoundException(string message, Exception innerException, string? callStack = null, string? queryParameters = null, string? bodyParameters = null) : base(message, innerException, callStack, queryParameters, bodyParameters)
        {
        }
    }
}
