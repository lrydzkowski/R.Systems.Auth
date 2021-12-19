using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Extensions.Hosting;
using Serilog.Formatting.Json;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace R.Systems.Auth.SharedKernel.Serilog
{
    public static class SerilogConfiguration
    {
        public static ReloadableLogger CreateBootstrapLogger()
        {
            return new LoggerConfiguration()
                .WriteTo.Console(
                    formatter: new ExpressionTemplate(
                        "[{@t:HH:mm:ss.fff} {@l:u3}] {@m}\n{@x}",
                        theme: TemplateTheme.Code
                    )
                )
                .CreateBootstrapLogger();
        }

        public static IHostBuilder UserSerilogWithStandardConfiguration(this IHostBuilder builder)
        {
            return builder.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .Enrich.WithExceptionDetails()
                    .ReadFrom.Configuration(context.Configuration)
                    .WriteTo.Console(
                        formatter: new ExpressionTemplate(
                            "[{@t:HH:mm:ss.fff} {@l:u3}] {@m}\n{@x}",
                            theme: TemplateTheme.Code
                        )
                    )
                    .WriteTo.File(
                        path: "logs/info-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 60,
                        rollOnFileSizeLimit: true,
                        fileSizeLimitBytes: 1000000,
                        formatter: new JsonFormatter()
                    );
            });
        }
    }
}
