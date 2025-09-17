namespace DbManager.Domain.Exceptions;

public class AlreadyExistsException : AppException
{
    private const string DEFAULT_MESSAGE = "Object with given params already exists.";
    private const string DEFAULT_MESSAGE_FORMAT = "The {0} with {1} already exists.";

    public AlreadyExistsException()
        : this(DEFAULT_MESSAGE)
    {
    }

    public AlreadyExistsException(string message)
        : base(message)
    {
    }

    public AlreadyExistsException(string resourceName, object resourceKey)
        : base(DEFAULT_MESSAGE_FORMAT, resourceName, resourceKey)
    {
    }

    public AlreadyExistsException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
