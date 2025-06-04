namespace TreeAPI.Domain.Exceptions
{
    public class SecureException : DomainException
    {
        public SecureException(string message, string? callStack = null, string? queryParameters = null, string? bodyParameters = null)
         : base(message, callStack, queryParameters, bodyParameters) { }

        public SecureException(string message, Exception innerException, string? callStack = null, string? queryParameters = null, string? bodyParameters = null)
            : base(message, innerException, callStack, queryParameters, bodyParameters) { }
    }
}
