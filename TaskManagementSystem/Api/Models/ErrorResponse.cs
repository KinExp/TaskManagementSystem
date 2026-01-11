namespace TaskManagement.Api.Models
{
    public class ErrorResponse
    {
        public int StatusCode { get; init; }
        public string Type { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
    }
}
