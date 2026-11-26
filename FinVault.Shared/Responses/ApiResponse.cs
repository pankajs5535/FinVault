
namespace FinVault.Shared.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public object? Errors { get; set; }
        public int StatusCode { get; set; }
        public string? TraceId { get; set; }

        public static ApiResponse<T> SuccessResponse(
            T? data,
            string message = "Request completed successfully.",
            int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = statusCode
            };
        }

        public static ApiResponse<T> FailureResponse(
            string message,
            int statusCode,
            object? errors = null,
            string? traceId = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode,
                TraceId = traceId
            };
        }
    }
}
