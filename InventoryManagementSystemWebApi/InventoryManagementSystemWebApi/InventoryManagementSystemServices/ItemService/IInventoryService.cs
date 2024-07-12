using InventoryManagementSystemData.Models;

namespace InventoryManagementSystemServices
{
    public interface IInventoryService
    {
        Task AddItemAsync(string name, int quantity, decimal price, string supplier);

        Task UpdateItemAsync(int id, int? quantity = null, decimal? price = null, string supplier = null);

        Task DeleteItemAsync(int id);

        Task<IEnumerable<Item>> SearchItemsByName(string name);

        Task<IEnumerable<Item>> GenerateInventoryReportAsync();
    }
}
