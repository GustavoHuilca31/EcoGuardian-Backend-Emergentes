namespace EcoGuardian_Backend.IAM.Domain.Model.Commands;

public record SyncUserFromAuth0Command(string Auth0UserId, string Email, string Role);
