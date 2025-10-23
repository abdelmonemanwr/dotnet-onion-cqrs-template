namespace MyProject.Api.Middleware
{
    public class BusinessException : Exception
    {
        public object ErrorDetails { get; }

        public BusinessException(string message, object errorDetails) : base(message)
        {
            ErrorDetails = errorDetails;
        }
    }
}
