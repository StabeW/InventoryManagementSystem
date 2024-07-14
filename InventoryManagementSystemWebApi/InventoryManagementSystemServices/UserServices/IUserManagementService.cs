using InventoryManagementSystemData.Models;
using InventoryManagementSystemDTOs.UsersDto;

using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystemServices.UserServices
{
    public interface IUserManagementService
    {
        Task<ServiceResult<User>> FindUserByIdAsync(string userId);

        Task<ServiceResult<bool>> ChangeUserRoleAsync(User user, ChangeUserRoleDto changeUserRoleDto);

        Task<ServiceResult<IdentityResult>> CreateUserAsync(User user, string password);

        Task<ServiceResult<bool>> AddUserToRoleAsync(User user, string role);

        Task<ServiceResult<User>> FindByNameAsync(string username);

        Task<ServiceResult<bool>> CheckPasswordAsync(User user, string password);
    }
}