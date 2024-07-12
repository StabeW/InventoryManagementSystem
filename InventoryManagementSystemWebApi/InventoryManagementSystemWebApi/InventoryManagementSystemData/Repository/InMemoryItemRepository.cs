using InventoryManagementSystemData.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystemData
{
    public class InMemoryItemRepository : IItemRepository
    {
        private readonly InventoryManagementSystemDbContext context;

        public InMemoryItemRepository(InventoryManagementSystemDbContext context)
        {
            this.context = context;
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            return await context.Items.FindAsync(id);
        }

        public async Task<Item> GetItemByNameAsync(string name)
        {
            return await context.Items.FirstOrDefaultAsync(i => i.Name == name);
        }

        public async Task AddItemAsync(string name, int quantity, decimal price, string supplier)
        {
            var newItem = new Item
            {
                Name = name,
                Quantity = quantity,
                Price = price,
                Supplier = supplier
            };

            await context.Items.AddAsync(newItem);
            await context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(int id, int? quantity, decimal? price, string supplier)
        {
            var item = await context.Items.FindAsync(id);
            if (item != null)
            {
                if (quantity.HasValue)
                    item.Quantity = quantity.Value;
                if (price.HasValue)
                    item.Price = price.Value;
                if (!string.IsNullOrEmpty(supplier))
                    item.Supplier = supplier;

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await context.Items.FindAsync(id);
            if (item != null)
            {
                context.Items.Remove(item);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Item>> SearchItemsByNameAsync(string name)
        {
            return await context.Items
                .Where(i => EF.Functions.Like(i.Name, $"%{name}%"))
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await context.Items.ToListAsync();
        }
    }
}
