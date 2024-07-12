using InventoryManagementSystemData.Common;
using InventoryManagementSystemServices.RoleService;
using InventoryManagementSystemData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using InventoryManagementSystemDTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventoryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserManagementService userManagementService;
        private readonly UserManager<User> userManager;

        public UserController(IUserManagementService userManagementService, UserManager<User> userManager)
        {
            this.userManagementService = userManagementService;
            this.userManager = userManager;
        }

        [HttpPost("change-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleDTO model)
        {
            var user = await userManagementService.FindUserByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound($"User with ID '{model.UserId}' not found.");
            }

            string newRoleName = null;

            switch (model.NewRole.ToLower())
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

            var result = await userManagementService.ChangeUserRoleAsync(user, newRoleName);

            if (result)
            {
                return Ok($"Role changed successfully for user '{user.UserName}'.");
            }
            else
            {
                return BadRequest($"Failed to change role for user '{user.UserName}'.");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] User model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.PasswordHash))
            {
                return BadRequest("Invalid user data.");
            }

            var result = await userManager.CreateAsync(model, model.PasswordHash);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(model, GlobalConstants.UserRole);
                return Ok("User created successfully.");
            }

            return BadRequest("Failed to create user.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var roles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

                authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YOUR_SECRET_KEY"));
            var token = new JwtSecurityToken(
                issuer: "YOUR_ISSUER",
                audience: "YOUR_AUDIENCE",
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

    }
}
