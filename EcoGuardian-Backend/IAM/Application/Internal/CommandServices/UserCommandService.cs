using EcoGuardian_Backend.IAM.Application.Internal.OutboundServices;
using EcoGuardian_Backend.IAM.Domain.Model.Aggregates;
using EcoGuardian_Backend.IAM.Domain.Model.Commands;
using EcoGuardian_Backend.IAM.Domain.Respositories;
using EcoGuardian_Backend.IAM.Domain.Services;
using EcoGuardian_Backend.Shared.Domain.Repositories;

namespace EcoGuardian_Backend.IAM.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IUnitOfWork unitOfWork)
    : IUserCommandService
{
    public async Task<(User user, string token)> Handle(SignInCommand command)
    {
        throw new NotSupportedException("Sign-in endpoint is deprecated. Please use Auth0 for authentication.");
    }

    public async Task<User?> Handle(SignUpCommand command)
    {
        throw new NotSupportedException("Sign-up endpoint is deprecated. Please use Auth0 for user registration.");
    }

    public async Task Handle(UpdateRoleCommand command)
    {
        var user = await userRepository.GetByIdAsync(command.UserId);
        if (user == null)
            throw new Exception("User not found");

        user.UpdateRoleId(command.RoleId);
        try
        {
            userRepository.Update(user);
            await unitOfWork.CompleteAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"An error occurred while updating user role: {e.Message}");
        }
    }

    public async Task<User> Handle(SyncUserFromAuth0Command command)
    {
        var existingUser = await userRepository.FindByAuth0UserIdAsync(command.Auth0UserId);
        if (existingUser != null)
            return existingUser;

        var role = await userRoleRepository.FindByRoleNameAsync(command.Role);
        if (role == null)
            throw new Exception($"Role {command.Role} not found");

        var user = new User(command.Email, role.Id, command.Auth0UserId);

        try
        {
            await userRepository.AddAsync(user);
            await unitOfWork.CompleteAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"An error occurred while syncing user from Auth0: {e.Message}");
        }

        return user;
    }
}
