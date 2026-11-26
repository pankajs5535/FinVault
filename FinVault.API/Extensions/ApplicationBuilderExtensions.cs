using FinVault.API.Middleware;

namespace FinVault.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();
            return app;
        }
    }
}
