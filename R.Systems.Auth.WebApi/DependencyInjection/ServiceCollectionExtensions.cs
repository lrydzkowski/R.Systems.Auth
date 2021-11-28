using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using R.Systems.Auth.Core.DependencyInjection;
using R.Systems.Auth.Infrastructure.Db.DependencyInjection;
using R.Systems.Auth.SharedKernel.DependencyInjection;
using R.Systems.Auth.WebApi.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace R.Systems.Auth.WebApi.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutomaticServices();
            services.AddSharedKernelServices();
            services.AddCoreServices();
            services.AddInfrastructureDbServices(configuration["DbConnectionString"]);
            services.AddSettingsServices(configuration);
            services.AddJwtServices(GetPublicKeyPemFile(configuration), GetJWTClockSkew(configuration));
            services.AddSwaggerServices();
        }

        private static void AddSettingsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.PropertyName));
        }

        private static string GetPublicKeyPemFile(IConfiguration configuration)
        {
            return File.ReadAllText(configuration["Jwt:PublicKeyPemFilePath"]);
        }

        private static double? GetJWTClockSkew(IConfiguration configuration)
        {
            bool clockSkewParsingResult = double.TryParse(configuration["Jwt:ClockSkewInSeconds"], out double clockSkew);
            if (!clockSkewParsingResult)
            {
                return null;
            }
            return clockSkew;
        }

        private static void AddJwtServices(this IServiceCollection services, string publicKeyPem, double? jwtClockSkew)
        {
            services.AddAuthentication(AddAuthenticationAction)
                .AddJwtBearer(AddJwtBearerAction(publicKeyPem, jwtClockSkew));
        }

        private static void AddAuthenticationAction(AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }

        private static Action<JwtBearerOptions> AddJwtBearerAction(string publicKeyPem, double? jwtClockSkew)
        {
            return new Action<JwtBearerOptions>(options =>
            {
                RSA rsa = RSA.Create();
                rsa.ImportFromPem(publicKeyPem.ToCharArray());
                TokenValidationParameters tokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new RsaSecurityKey(rsa)
                };
                if (jwtClockSkew != null)
                {
                    tokenValidationParameters.ClockSkew = TimeSpan.FromSeconds((double)jwtClockSkew);
                }
                options.TokenValidationParameters = tokenValidationParameters;
            });
        }

        private static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "R.Systems.Auth", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
        }
    }
}
