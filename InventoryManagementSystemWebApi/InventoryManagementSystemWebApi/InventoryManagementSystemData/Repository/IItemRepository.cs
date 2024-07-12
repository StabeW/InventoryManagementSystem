using InventoryManagementSystemData.Models;

namespace InventoryManagementSystemData
{
    public interface IItemRepository
    {
        Task<Item> GetItemByIdAsync(int id);

        Task<Item> GetItemByNameAsync(string name);

        Task AddItemAsync(string name, int quantity, decimal price, string supplier);

        Task UpdateItemAsync(int id, int? quantity, decimal? price, string supplier);

        Task DeleteItemAsync(int id);

        Task<IEnumerable<Item>> SearchItemsByNameAsync(string name);

        Task<IEnumerable<Item>> GetAllItemsAsync();
    }
}
