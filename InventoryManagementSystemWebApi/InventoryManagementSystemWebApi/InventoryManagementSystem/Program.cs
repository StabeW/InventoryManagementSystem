using InventoryManagementSystemData.Common;
using InventoryManagementSystemServices.RoleService;

namespace InventoryManagementSystemWebApi
{
    public class Program
    {
        private static IUserManagementService userManagementService;

        public static async Task Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();


            await ExecuteConsoleCommand(args);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static readonly ILogger logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }).CreateLogger<Program>();

        private static async Task ExecuteConsoleCommand(string[] args)
        {
            if (args.Length < 3)
            {
                logger.LogWarning("Usage: ChangeUserRole <userId> <newRole>");
                return;
            }

            string userId = args[1];
            string newRole = args[2];

            var user = await userManagementService.FindUserByIdAsync(userId);

            if (user == null)
            {
                logger.LogWarning($"User with ID '{userId}' not found.");
                return;
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
                    logger.LogError("Invalid role specified.");
                    return;
            }

            var result = await userManagementService.ChangeUserRoleAsync(user, newRoleName);

            if (result)
            {
                logger.LogInformation($"Role changed successfully for user '{user.UserName}'.");
            }
            else
            {
                logger.LogError($"Failed to change role for user '{user.UserName}'.");
            }
        }
    }
}
