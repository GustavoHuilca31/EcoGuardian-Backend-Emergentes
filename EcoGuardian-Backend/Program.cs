using EcoGuardian_Backend.IAM.Infrastructure.Pipeline.Middleware.Extensions;
using EcoGuardian_Backend.Shared.Application.Internal.CloudinaryStorage.Configuration;
using EcoGuardian_Backend.Shared.Application.IOC;
using EcoGuardian_Backend.Shared.Infrastructure.IOC;
using EcoGuardian_Backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using EcoGuardian_Backend.Shared.Interfaces.IOC;
using EcoGuardian_Backend.Shared.Application.Internal.EventHandler;
using EcoGuardian_Backend.IAM.Infrastructure.Auth0.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
StripeConfiguration.ApiKey = builder.Configuration["Stripe:ApiKey"];

builder.Services.Configure<Auth0Settings>(builder.Configuration.GetSection("Auth0"));

var auth0Settings = builder.Configuration.GetSection("Auth0").Get<Auth0Settings>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{auth0Settings!.Domain}/";
        options.Audience = auth0Settings.Audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://{auth0Settings.Domain}/",
            ValidateAudience = true,
            ValidAudience = auth0Settings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.SetUpStorageService();
builder.Services.AddInfrastructureDependencies(builder, configuration);
builder.Services.AddApplicationDependencies();
builder.Services.AddInterfaceDependencies(builder);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(9080); 
});



var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    services.OnStart();
}


// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoGuardian API v1");
        c.OAuthClientId(auth0Settings!.ClientId);
        c.OAuthUsePkce();
        c.OAuthScopes("openid", "profile", "email");
    });

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseDeviceAuthorization();
app.MapControllers();
app.UseRequestAuthorization();

app.Run();

