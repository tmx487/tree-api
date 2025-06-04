namespace TreeAPI.Domain.Exceptions
{
    public class InternalServerErrorException : DomainException
    {
        public InternalServerErrorException(string message, string? callStack = null, string? queryParameters = null, string? bodyParameters = null)
            : base(message, callStack, queryParameters, bodyParameters) { }

        public InternalServerErrorException(string message, Exception innerException, string? callStack = null, string? queryParameters = null, string? bodyParameters = null)
            : base(message, innerException, callStack ?? innerException.StackTrace, queryParameters, bodyParameters) { }
    }
}
