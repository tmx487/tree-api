
namespace TreeAPI.Domain.Exceptions
{
    public class NodeHasChildrenException : SecureException
    {
        public NodeHasChildrenException(string message, string? callStack = null, string? queryParameters = null, string? bodyParameters = null) : base(message, callStack, queryParameters, bodyParameters)
        {
        }

        public NodeHasChildrenException(string message, Exception innerException, string? callStack = null, string? queryParameters = null, string? bodyParameters = null) : base(message, innerException, callStack, queryParameters, bodyParameters)
        {
        }
    }
}
