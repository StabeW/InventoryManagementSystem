using InventoryManagementSystemData.Common;
using InventoryManagementSystemServices.RoleService;
using Microsoft.AspNetCore.Mvc;

public class UserController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;

    public UserController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<IActionResult> ChangeUserRole(string userId, string newRole)
    {
        var user = await _userManagementService.FindUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound($"User with ID '{userId}' not found.");
        }

        string newRoleName = null;

        switch (newRole.ToLower())
        {
            case "admin":
                newRoleName = GlobalConstants.AdminRole;
                break;
            case "user":
                newRoleName = GlobalConstants.UserRole;
                break;
            default:
                return BadRequest("Invalid role specified.");
        }

        var result = await _userManagementService.ChangeUserRoleAsync(user, newRoleName);

        if (result)
        {
            return Ok($"Role changed successfully for user '{user.UserName}'.");
        }
        else
        {
            return BadRequest($"Failed to change role for user '{user.UserName}'.");
        }
    }
}
