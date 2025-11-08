using EcoGuardian_Backend.IAM.Domain.Model.Entities;
using EcoGuardian_Backend.IAM.Domain.Model.ValueObjects;
using EcoGuardian_Backend.Shared.Domain.Repositories;

namespace EcoGuardian_Backend.IAM.Domain.Respositories;

public interface IUserRoleRepository : IBaseRepository<UserRole>
{
    Task<bool> ExistsUserRole(string role);
    Task<UserRole?> FindByRoleNameAsync(string roleName);
}