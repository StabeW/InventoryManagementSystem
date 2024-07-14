using InventoryManagementSystemCommon;
using InventoryManagementSystemData.Models;
using InventoryManagementSystemDTOs.UsersDto;
using InventoryManagementSystemServices;
using InventoryManagementSystemServices.UserServices;

using Microsoft.AspNetCore.Identity;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<User> userManager;

    public UserManagementService(UserManager<User> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<ServiceResult<User>> FindUserByIdAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        return user != null 
            ? ServiceResult<User>.Success(user) 
            : ServiceResult<User>.NotFound(string.Format(ResponseMessageConstants.UserNotFoundErrorMessage, userId));
    }

    public async Task<ServiceResult<bool>> ChangeUserRoleAsync(User user, ChangeUserRoleDto model)
    {
        var currentRoles = await userManager.GetRolesAsync(user);

        await userManager.RemoveFromRolesAsync(user, currentRoles);

        var result = await userManager.AddToRoleAsync(user, model.NewRole);

        return result.Succeeded 
            ? ServiceResult<bool>.Success(true) 
            : ServiceResult<bool>.BadRequest(ResponseMessageConstants.FailedRoleChangeErrorMessage);
    }

    public async Task<ServiceResult<IdentityResult>> CreateUserAsync(User user, string password)
    {
        var result = await userManager.CreateAsync(user, password);

        return result.Succeeded 
            ? ServiceResult<IdentityResult>.Success(result) 
            : ServiceResult<IdentityResult>.BadRequest(ResponseMessageConstants.FailedUserCreateErrorMessage);
    }

    public async Task<ServiceResult<bool>> AddUserToRoleAsync(User user, string role)
    {
        var result = await userManager.AddToRoleAsync(user, role);

        return result.Succeeded 
            ? ServiceResult<bool>.Success(true) 
            : ServiceResult<bool>.BadRequest(ResponseMessageConstants.FailedRoleAssignmentErrorMessage);
    }

    public async Task<ServiceResult<User>> FindByNameAsync(string username)
    {
        var user = await userManager.FindByNameAsync(username);

        return user != null 
            ? ServiceResult<User>.Success(user) 
            : ServiceResult<User>.NotFound(string.Format(ResponseMessageConstants.FindUserByUsernameErrorMessage, username));
    }

    public async Task<ServiceResult<bool>> CheckPasswordAsync(User user, string password)
    {
        var result = await userManager.CheckPasswordAsync(user, password);

        return result 
            ? ServiceResult<bool>.Success(true) 
            : ServiceResult<bool>.BadRequest(ResponseMessageConstants.IncorrectPasswordErrorMessage);
    }
}
