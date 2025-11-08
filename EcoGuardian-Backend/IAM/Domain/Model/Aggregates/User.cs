using EcoGuardian_Backend.IAM.Domain.Model.Commands;

namespace EcoGuardian_Backend.IAM.Domain.Model.Aggregates;

public class User
{

    public int Id { get; }

    public string Email { get; private set; }

    public string Password { get; private set; }

    public int RoleId { get; private set; }

    public string? Auth0UserId { get; private set; }

    public User()
    {
        Email = string.Empty;
        Password = string.Empty;
        RoleId = 0;
    }


    public User(SignUpCommand command)
    {
        Email = command.Email;
        Password = command.Password;
        RoleId = command.RoleId;
    }

    public User(string email, int roleId, string auth0UserId)
    {
        Email = email;
        Password = string.Empty;
        RoleId = roleId;
        Auth0UserId = auth0UserId;
    }

    public void UpdateRoleId(int roleId)
    {
        RoleId = roleId;
    }

    public void UpdatePassword(string password)
    {
        Password = password;
    }

    public void SetAuth0UserId(string auth0UserId)
    {
        Auth0UserId = auth0UserId;
    }
}