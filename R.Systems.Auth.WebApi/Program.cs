using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using R.Systems.Shared.WebApi.Serilog;
using Serilog;
using System;

namespace R.Systems.Auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = SerilogConfiguration.CreateBootstrapLogger();
            Log.Information("Starting up!");
            try
            {
                CreateHostBuilder(args).Build().Run();
                Log.Information("Stopped cleanly");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UserSerilogWithStandardConfiguration()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
