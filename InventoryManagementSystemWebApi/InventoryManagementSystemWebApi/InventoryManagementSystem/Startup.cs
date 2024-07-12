using InventoryManagementSystemData;
using InventoryManagementSystemData.Models;
using InventoryManagementSystemServices;
using InventoryManagementSystemServices.ItemService;
using InventoryManagementSystemServices.RoleService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystemWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IItemRepository, InMemoryItemRepository>();
            services.AddTransient<IInventoryService, InventoryService>();

            services.AddScoped<IUserManagementService, UserManagementService>();

            services.AddHttpContextAccessor();

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<InventoryManagementSystemDbContext>()
                .AddDefaultTokenProviders();

            services.AddLogging(logging =>
            {
                logging.AddConfiguration(Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
            });

            services.AddDbContext<InventoryManagementSystemDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
