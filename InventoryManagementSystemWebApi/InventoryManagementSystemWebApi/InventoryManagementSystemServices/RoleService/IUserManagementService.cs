using InventoryManagementSystemData.Models;

namespace InventoryManagementSystemServices.RoleService
{
    public interface IUserManagementService
    {
        Task<User> FindUserByIdAsync(string userId);

        Task<bool> ChangeUserRoleAsync(User user, string newRoleName);
    }
}