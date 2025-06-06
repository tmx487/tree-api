namespace TreeAPI.Domain.Exceptions
{
    public class NodeNotFoundException : SecureException
    {
        public NodeNotFoundException(
            string message,
            string? callStack = null,
            string? queryParameters = null,
            string? bodyParameters = null) : base(message, callStack, queryParameters, bodyParameters)
        {
        }

        public NodeNotFoundException(
            string message,
            Exception innerException,
            string? callStack = null,
            string? queryParameters = null,
            string? bodyParameters = null) : base(message, innerException, callStack, queryParameters, bodyParameters)
        {
        }
    }
}
