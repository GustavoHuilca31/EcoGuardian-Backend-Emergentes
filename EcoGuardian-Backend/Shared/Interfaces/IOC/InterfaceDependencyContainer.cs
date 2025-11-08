using EcoGuardian_Backend.IAM.Infrastructure.Auth0.Configuration;
using EcoGuardian_Backend.Shared.Application.Helper;
using EcoGuardian_Backend.Shared.Interfaces.ASP.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EcoGuardian_Backend.Shared.Interfaces.IOC;

public static class InterfaceDependencyContainer
{
    public static IServiceCollection AddInterfaceDependencies(this IServiceCollection services, WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "EcoGuardian API", Version = "v1" });
            c.OperationFilter<FileUploadOperationFilter>();

            var auth0Settings = builder.Configuration.GetSection("Auth0").Get<Auth0Settings>();

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"https://{auth0Settings!.Domain}/authorize"),
                        TokenUrl = new Uri($"https://{auth0Settings.Domain}/oauth/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID" },
                            { "profile", "Profile" },
                            { "email", "Email" },
                            { "read:metrics", "Read Metrics" },
                            { "write:metrics", "Write Metrics" }
                        }
                    }
                },
                In = ParameterLocation.Header,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        builder.Services.Configure<Auth0Settings>(builder.Configuration.GetSection("Auth0"));
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", corsBuilder =>
            {
                corsBuilder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                
            });
        });
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers( options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));
        return services;
    }
}