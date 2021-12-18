using Microsoft.AspNetCore.Builder;

namespace R.Systems.Auth.SharedKernel.Middlewares
{
    public static class MiddlewaresExtensions
    {
        public static void UseSharedKernelExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
