namespace EcoGuardian_Backend.IAM.Infrastructure.Auth0.Configuration;

public class Auth0Settings
{
    public string Domain { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public Auth0ManagementSettings Management { get; set; } = new();
}

public class Auth0ManagementSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
