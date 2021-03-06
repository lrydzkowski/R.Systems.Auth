using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.FunctionalTests.Services;
using R.Systems.Auth.Infrastructure.Db;
using R.Systems.Shared.Core.Interfaces;

namespace R.Systems.Auth.FunctionalTests.Initializers;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        OverrideConfiguration(builder);
        builder.ConfigureServices(services =>
        {
            ReplaceIRsaKeysLoader(services);
            ReplaceDbContext(services);
        });
    }

    private void OverrideConfiguration(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    ["Jwt:PrivateKeyPemFilePath"] = "private.pem",
                    ["Jwt:PublicKeyPemFilePath"] = "public.pem",
                    ["User:MaxNumOfIncorrectLoginsBeforeBlock"] = UserSettings.MaxNumOfIncorrectLoginsBeforeBlock.ToString(),
                    ["User:BlockDurationInMinutes"] = UserSettings.BlockDurationInMinutes.ToString(CultureInfo.InvariantCulture)
                }
            );
        });
    }

    private void ReplaceIRsaKeysLoader(IServiceCollection services)
    {
        RemoveService(services, typeof(IRsaKeys));
        services.AddSingleton<IRsaKeys, EmbeddedRsaKeys>();
    }

    private void ReplaceDbContext(IServiceCollection services)
    {
        RemoveService(services, typeof(DbContextOptions<AuthDbContext>));
        string inMemoryDatabaseName = Guid.NewGuid().ToString();
        services.AddDbContext<AuthDbContext>(options => options.UseInMemoryDatabase(inMemoryDatabaseName));
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        IServiceProvider scopedServiceProvider = scope.ServiceProvider;
        AuthDbContext dbContext = GetService<AuthDbContext>(scopedServiceProvider);
        IPasswordHasher passwordHasher = GetService<IPasswordHasher>(scopedServiceProvider);

        dbContext.Database.EnsureCreated();
        DbInitializer.InitData(dbContext, passwordHasher);
    }

    private void RemoveService(IServiceCollection services, Type serviceType)
    {
        ServiceDescriptor? serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == serviceType);
        if (serviceDescriptor != null)
        {
            services.Remove(serviceDescriptor);
        }
    }

    private T GetService<T>(IServiceProvider serviceProvider)
    {
        T? service = serviceProvider.GetService<T>();
        if (service == null)
        {
            throw new Exception($"Service {nameof(T)} doesn't exist.");
        }
        return service;
    }
}
