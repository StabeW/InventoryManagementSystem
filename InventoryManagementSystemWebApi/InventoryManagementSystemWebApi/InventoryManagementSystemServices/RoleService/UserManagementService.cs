using InventoryManagementSystemData.Models;
using InventoryManagementSystemServices.RoleService;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Service class for managing user operations, including finding users by ID and changing their roles.
/// </summary>
public class UserManagementService : IUserManagementService
{
    private readonly UserManager<User> userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserManagementService"/> class.
    /// </summary>
    /// <param name="userManager">The user manager instance for managing users.</param>
    public UserManagementService(UserManager<User> userManager)
    {
        this.userManager = userManager;
    }

    /// <summary>
    /// Finds a user by their ID asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user to find.</param>
    /// <returns>The user with the specified ID, or null if no such user exists.</returns>
    public async Task<User> FindUserByIdAsync(string userId)
    {
        return await userManager.FindByIdAsync(userId);
    }

    /// <summary>
    /// Changes the role of a specified user asynchronously.
    /// </summary>
    /// <param name="user">The user whose role is to be changed.</param>
    /// <param name="newRoleName">The name of the new role to assign to the user.</param>
    /// <returns>True if the role change was successful; otherwise, false.</returns>
    public async Task<bool> ChangeUserRoleAsync(User user, string newRoleName)
    {
        if (user == null)
            return false;

        var currentRoles = await userManager.GetRolesAsync(user);

        await userManager.RemoveFromRolesAsync(user, currentRoles);

        var result = await userManager.AddToRoleAsync(user, newRoleName);

        return result.Succeeded;
    }
}
