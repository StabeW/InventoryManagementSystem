using Microsoft.AspNetCore.Identity;

using InventoryManagementSystemData.Models;
using InventoryManagementSystem.Infrastructure;
using InventoryManagementSystemDTOs.ConfigurationsDto;
using InventoryManagementSystemServices.InventoryServices;
using InventoryManagementSystemData.Repositories;
using InventoryManagementSystemServices.UserServices;
using InventoryManagementSystemCommon;
using Microsoft.OpenApi.Models;

namespace InventoryManagementSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwagger();

            builder.Services.AddControllers();

            builder.Services.AddApplicationServices();

            builder.Services.AddIdentity();

            builder.Services.AddJwtAuthentication(builder.Services.GetApplicationSettings(builder.Configuration));

            builder.Services.AddDatabase(builder.Configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoryManagementApi.V1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                await SeedRolesAndAdminAsync(services);
            }

            app.Run();
        }

        private static async Task SeedRolesAndAdminAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<User>>();

            string[] roleNames = { GlobalConstants.AdminRole, GlobalConstants.UserRole};
            string adminEmail = "admin@abv.bg";
            string adminPassword = "Admin123!";

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdminUser = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                };
                var result = await userManager.CreateAsync(newAdminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdminUser, GlobalConstants.AdminRole);
                }
            }
        }
    }
}