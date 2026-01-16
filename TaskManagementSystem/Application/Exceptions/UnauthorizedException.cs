namespace TaskManagement.Application.Exceptions
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message) 
            : base(message) 
        { 
        }
    }
}
