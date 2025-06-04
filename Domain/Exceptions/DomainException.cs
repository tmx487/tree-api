namespace TreeAPI.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        public long EventId { get; }
        public string? CallStatck { get; }
        public string? QueryParameters { get; }
        public string? BodyParameters { get; }

        protected DomainException(
            string message,
            string? callStack = null,
            string? queryParams = null,
            string? bodyParams = null) : base(message)
        {
            EventId = DateTime.UtcNow.Ticks;
            CallStatck = callStack;
            QueryParameters = queryParams;
            BodyParameters = bodyParams;
        }
        protected DomainException(
            string message,
            Exception innerException,
            string? callStack = null,
            string? queryParams = null,
            string? bodyParams = null) : base(message, innerException)
        {
            EventId = DateTime.UtcNow.Ticks;
            CallStatck = callStack;
            QueryParameters = queryParams;
            BodyParameters = bodyParams;
        }
    }
}
