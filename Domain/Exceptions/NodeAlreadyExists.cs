namespace TreeAPI.Domain.Exceptions
{
    public class NodeAlreadyExists : SecureException
    {
        public NodeAlreadyExists(
            string message,
            string? callStack = null,
            string? queryParameters = null,
            string? bodyParameters = null)
            : base(message, callStack, queryParameters, bodyParameters)
        {
        }

        public NodeAlreadyExists(
            string message,
            Exception innerException,
            string? callStack = null,
            string? queryParameters = null,
            string? bodyParameters = null)
            : base(message, innerException, callStack, queryParameters, bodyParameters)
        {
        }
    }
}
