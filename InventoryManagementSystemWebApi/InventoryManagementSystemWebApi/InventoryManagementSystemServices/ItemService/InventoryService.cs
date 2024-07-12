using InventoryManagementSystemData.Models;
using InventoryManagementSystemData;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace InventoryManagementSystemServices.ItemService
{
    /// <summary>
    /// Service class for managing inventory operations such as adding, updating, deleting, and searching items.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly IItemRepository itemRepository;
        private readonly ILogger<InventoryService> logger;
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryService"/> class.
        /// </summary>
        /// <param name="itemRepository">The repository providing access to inventory items.</param>
        /// <param name="logger">The logger instance for logging messages.</param>
        public InventoryService(IItemRepository itemRepository, ILogger<InventoryService> logger, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.itemRepository = itemRepository;
            this.logger = logger;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Adds a new item to the inventory asynchronously.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="quantity">The quantity of the item.</param>
        /// <param name="price">The price of the item.</param>
        /// <param name="supplier">The supplier of the item.</param>
        public async Task AddItemAsync(string name, int quantity, decimal price, string supplier)
        {
            try
            {
                var existingItem = await itemRepository.GetItemByNameAsync(name);

                if (existingItem != null)
                {
                    logger.LogWarning($"Item with ID {existingItem.Id} already exists.");
                    return;
                }

                await itemRepository.AddItemAsync(name, quantity, price, supplier);
                logger.LogInformation("Item added successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while adding the item.");
            }
        }

        /// <summary>
        /// Updates an existing item asynchronously.
        /// </summary>
        /// <param name="id">The ID of the item to update.</param>
        /// <param name="quantity">The new quantity of the item (optional).</param>
        /// <param name="price">The new price of the item (optional).</param>
        /// <param name="supplier">The new supplier of the item (optional).</param>
        public async Task UpdateItemAsync(int id, int? quantity = null, decimal? price = null, string supplier = null)
        {
            try
            {
                var item = await itemRepository.GetItemByIdAsync(id);

                if (item == null)
                {
                    logger.LogWarning($"Item with ID {id} not found.");
                    return;
                }

                await itemRepository.UpdateItemAsync(id, quantity, price, supplier);
                logger.LogInformation("Item updated successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while updating the item with ID {id}.");
            }
        }

        /// <summary>
        /// Deletes an item from the inventory asynchronously.
        /// </summary>
        /// <param name="id">The ID of the item to delete.</param>
        public async Task DeleteItemAsync(int id)
        {
            try
            {
                var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var user = await userManager.FindByIdAsync(userId);

                if (user == null || !(await userManager.IsInRoleAsync(user, "Admin")))
                {
                    logger.LogWarning("Unauthorized attempt to delete item.");
                    return;
                }

                var item = await itemRepository.GetItemByIdAsync(id);

                if (item != null)
                {
                    await itemRepository.DeleteItemAsync(id);
                    logger.LogInformation("Item deleted successfully.");
                }
                else
                {
                    logger.LogWarning($"Item with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while deleting the item with ID {id}.");
            }
        }

        /// <summary>
        /// Searches for items in the inventory asynchronously whose names contain the specified text.
        /// </summary>
        /// <param name="name">The text to search for in the item names.</param>
        /// <returns>A collection of items whose names contain the specified text.</returns>
        public async Task<IEnumerable<Item>> SearchItemsByName(string name)
        {
            try
            {
                return await itemRepository.SearchItemsByNameAsync(name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while searching for items with name containing '{name}'.");
                return Enumerable.Empty<Item>();
            }
        }

        /// <summary>
        /// Generates a report of the current inventory status asynchronously.
        /// </summary>
        /// <returns>A collection of items representing the current inventory status.</returns>
        public async Task<IEnumerable<Item>> GenerateInventoryReportAsync()
        {
            try
            {
                var items = await itemRepository.GetAllItemsAsync();
                var report = items.Select(item => new Item
                {
                    Id = item.Id,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Supplier = item.Supplier,
                });

                logger.LogInformation("Inventory report generated successfully.");
                return report;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while generating the inventory report.");
                return Enumerable.Empty<Item>();
            }
        }
    }
}