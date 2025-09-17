namespace DbManager.Domain.Exceptions
{
    public class BadRequestException : AppException
    {
        private const string DEFAULT_MESSAGE = "Bad request";

        public BadRequestException()
            : this(DEFAULT_MESSAGE)
        {
        }

        public BadRequestException(string message)
            : base(message)
        {
        }

        public BadRequestException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
