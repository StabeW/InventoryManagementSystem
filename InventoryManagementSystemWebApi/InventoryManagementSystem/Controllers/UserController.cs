using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

using InventoryManagementSystemData.Models;
using InventoryManagementSystemDTOs.ConfigurationsDto;
using InventoryManagementSystemDTOs.UsersDto;
using InventoryManagementSystemServices.UserServices;
using InventoryManagementSystemCommon;

namespace InventoryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserManagementService userManagementService;
        private readonly UserManager<User> userManager;
        private readonly ApplicationSettings appSettings;

        public UserController(
            IUserManagementService userManagementService,
            UserManager<User> userManager,
            IOptions<ApplicationSettings> appSettings)
        {
            this.userManagementService = userManagementService;
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var result = await userManagementService.FindUserByIdAsync(userId);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            else
            {
                return NotFound(result.ErrorMessage);
            }
        }

        [HttpPost("change-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleDto model)
        {
            var userResult = await userManagementService.FindUserByIdAsync(model.UserId);

            if (!userResult.IsSuccess)
            {
                return NotFound(userResult.ErrorMessage);
            }

            var changeRoleResult = await userManagementService.ChangeUserRoleAsync(userResult.Data, model);

            if (changeRoleResult.IsSuccess)
            {
                return Ok(string.Format(ResponseMessageConstants.SuccessfullyChangedUserRoleMessage));
            }
            else
            {
                return BadRequest(changeRoleResult.ErrorMessage);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseMessageConstants.InvalidUserDataErrorMessage);
            }

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
            };

            var createUserResult = await userManagementService.CreateUserAsync(user, model.Password);

            if (!createUserResult.IsSuccess)
            {
                return BadRequest(createUserResult.ErrorMessage);
            }

            return Ok(ResponseMessageConstants.SuccessfullyCreatedUserMessage);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var userResult = await userManagementService.FindByNameAsync(model.Username);

            if (!userResult.IsSuccess)
            {
                return BadRequest(userResult.ErrorMessage);
            }

            var checkPasswordResult = await userManagementService.CheckPasswordAsync(userResult.Data, model.Password);

            if (!checkPasswordResult.IsSuccess)
            {
                return BadRequest(checkPasswordResult.ErrorMessage);
            }

            var user = userResult.Data;
            var role = await userManager.GetRolesAsync(user);

            var options = new IdentityOptions();
            string token = GenerateJwtBearerToken(user, role, options);

            return Ok(new { token });
        }

        private string GenerateJwtBearerToken(User user, IList<string> role, IdentityOptions options)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("UserId", user.Id),
                new Claim(options.ClaimsIdentity.RoleClaimType, role.FirstOrDefault())
            }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return token;
        }
    }
}
