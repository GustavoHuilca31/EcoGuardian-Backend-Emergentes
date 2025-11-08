using EcoGuardian_Backend.IAM.Domain.Model.Aggregates;
using EcoGuardian_Backend.IAM.Domain.Model.Queries;
using EcoGuardian_Backend.IAM.Domain.Respositories;
using EcoGuardian_Backend.IAM.Domain.Services;

namespace EcoGuardian_Backend.IAM.Application.Internal.QueryServices;

public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    public async Task<User?> Handle(GetUserByIdQuery query)
    {
        return await userRepository.GetByIdAsync(query.Id);
    }

    public async Task<User?> Handle(GetUserByAuth0UserIdQuery query)
    {
        return await userRepository.FindByAuth0UserIdAsync(query.Auth0UserId);
    }
}