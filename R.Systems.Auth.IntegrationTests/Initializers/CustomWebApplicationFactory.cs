using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.Auth.Common.Interfaces;
using R.Systems.Auth.Db;
using R.Systems.Auth.IntegrationTests.Services;
using R.Systems.Auth.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace R.Systems.Auth.IntegrationTests.Initializers
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            OverrideConfiguration(builder);
            builder.ConfigureServices(services =>
            {
                ReplaceIPrivateKeyLoader(services);
                ReplaceDbContext(services);
            });
        }

        private void OverrideConfiguration(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["Jwt:AccessTokenLifeTimeInMinutes"] = "1"
                    }
                );
            });
        }

        private void ReplaceIPrivateKeyLoader(IServiceCollection services)
        {
            RemoveService(services, typeof(IPrivateKeyLoader));
            services.AddScoped<IPrivateKeyLoader, PrivateKeyEmbeddedLoader>();
        }

        private void ReplaceDbContext(IServiceCollection services)
        {
            RemoveService(services, typeof(DbContextOptions<AuthDbContext>));
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseInMemoryDatabase("users");
            });
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            using IServiceScope scope = serviceProvider.CreateScope();
            IServiceProvider scopedServiceProvider = scope.ServiceProvider;
            AuthDbContext dbContext = GetService<AuthDbContext>(scopedServiceProvider);
            IPasswordHasher? passwordHasher = GetService<IPasswordHasher>(scopedServiceProvider);

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
}
