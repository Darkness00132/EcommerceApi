namespace Application.Exceptions;

public sealed class ForbiddenException : Exception
{
    public ForbiddenException(string message = "You are not allowed to perform this action.")
        : base(message)
    {
    }
}
