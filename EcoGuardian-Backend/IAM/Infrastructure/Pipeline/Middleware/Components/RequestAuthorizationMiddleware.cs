using System.Security.Claims;
using EcoGuardian_Backend.IAM.Domain.Model.Commands;
using EcoGuardian_Backend.IAM.Domain.Respositories;
using EcoGuardian_Backend.IAM.Domain.Services;
using EcoGuardian_Backend.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace EcoGuardian_Backend.IAM.Infrastructure.Pipeline.Middleware.Components;

public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IUserRepository userRepository,
        IUserCommandService userCommandService)
    {
        Console.WriteLine("Entering InvokeAsync");

        if (context.Request.HttpContext.GetEndpoint() == null)
        {
            Console.WriteLine("Endpoint is null, skipping authorization");
            await next(context);
            return;
        }

        var allowAnonymous = context.Request.HttpContext.GetEndpoint()!.Metadata
            .Any(m => m.GetType() == typeof(AllowAnonymousAttribute));
        Console.WriteLine($"Allow Anonymous is {allowAnonymous}");

        if (allowAnonymous)
        {
            Console.WriteLine("Skipping authorization");
            await next(context);
            return;
        }

        Console.WriteLine("Entering authorization");

        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            Console.WriteLine("User not authenticated");
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var auth0UserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = context.User.FindFirst(ClaimTypes.Email)?.Value
                   ?? context.User.FindFirst("email")?.Value;
        var role = context.User.FindFirst(ClaimTypes.Role)?.Value
                  ?? context.User.FindFirst("role")?.Value
                  ?? context.User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value
                  ?? "Domestic";

        if (auth0UserId == null)
        {
            Console.WriteLine("Auth0UserId claim not found");
            throw new UnauthorizedAccessException("Invalid token claims");
        }

        Console.WriteLine($"Auth0UserId: {auth0UserId}, Email: {email}, Role: {role}");

        var user = await userRepository.FindByAuth0UserIdAsync(auth0UserId);

        if (user == null && email != null)
        {
            Console.WriteLine("User not found in DB, creating from Auth0");
            var syncCommand = new SyncUserFromAuth0Command(auth0UserId, email, role);
            user = await userCommandService.Handle(syncCommand);
        }

        if (user == null)
        {
            Console.WriteLine("Failed to sync user from Auth0");
            throw new UnauthorizedAccessException("User not found");
        }

        Console.WriteLine("Successful authorization. Updating Context...");
        context.Items["User"] = user;
        Console.WriteLine("Continuing with Middleware Pipeline");

        await next(context);
    }
}