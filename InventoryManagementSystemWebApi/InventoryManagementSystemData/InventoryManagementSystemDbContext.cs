using InventoryManagementSystemData.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystemData
{
    public class InventoryManagementSystemDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public InventoryManagementSystemDbContext(DbContextOptions<InventoryManagementSystemDbContext> options)
            : base(options) { }

        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}