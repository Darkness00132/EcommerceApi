namespace Application.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "Please login and try again!")
            : base(message)
        {
        }
    }
}
