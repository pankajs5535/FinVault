using FinVault.Shared.Responses;

namespace FinVault.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An unhandled exception occurred while processing the request.");
                if (context.Response.HasStarted)
                {
                  
                    throw;
                }


                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.FailureResponse(
                 "An unexpected error occurred.",
                 StatusCodes.Status500InternalServerError,
                 traceId: context.TraceIdentifier);

                await context.Response.WriteAsJsonAsync(response);
            }
        }

    }
}
