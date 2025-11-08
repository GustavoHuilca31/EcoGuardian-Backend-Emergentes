using EcoGuardian_Backend.IAM.Domain.Model.Aggregates;
using EcoGuardian_Backend.IAM.Domain.Model.Commands;

namespace EcoGuardian_Backend.IAM.Domain.Services;

public interface IUserCommandService
{
    Task<(User user, string token)> Handle(SignInCommand command);
    Task<User?> Handle(SignUpCommand command);
    Task Handle(UpdateRoleCommand command);
    Task<User> Handle(SyncUserFromAuth0Command command);
}