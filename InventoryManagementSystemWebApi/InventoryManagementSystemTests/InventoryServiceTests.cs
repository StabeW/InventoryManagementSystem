using InventoryManagementSystemData.Models;
using InventoryManagementSystemData.Repositories;
using InventoryManagementSystemServices.InventoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace InventoryManagementSystemTests
{
    public class InventoryServiceTests
    {
        private readonly Mock<IITemRepository> mockItemRepository;
        private readonly Mock<ILogger<ItemService>> mockLogger;
        private readonly Mock<UserManager<User>> mockUserManager;
        private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor;
        private readonly ItemService inventoryService;

        public InventoryServiceTests()
        {
            mockItemRepository = new Mock<IITemRepository>();
            mockLogger = new Mock<ILogger<ItemService>>();
            mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            inventoryService = new ItemService(
                mockItemRepository.Object,
                mockLogger.Object,
                mockUserManager.Object,
                mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task AddItemAsync_WithNewItem_ShouldAddItem()
        {
            string name = "Test Item";
            int quantity = 10;
            decimal price = 100.0m;
            string supplier = "Test Supplier";

            mockItemRepository.Setup(r => r.GetItemByNameAsync(name))
                               .ReturnsAsync((Item)null);

            await inventoryService.AddItemAsync(name, quantity, price, supplier);

            mockItemRepository.Verify(r => r.GetItemByNameAsync(name), Times.Once);
            mockItemRepository.Verify(r => r.AddItemAsync(name, quantity, price, supplier), Times.Once);
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task AddItemAsync_WithExistingItem_ShouldLogWarning()
        {
            string name = "Test Item";
            int quantity = 10;
            decimal price = 100.0m;
            string supplier = "Test Supplier";

            var existingItem = new Item { Id = 1, Name = name };

            mockItemRepository.Setup(r => r.GetItemByNameAsync(name))
                               .ReturnsAsync(existingItem);

            await inventoryService.AddItemAsync(name, quantity, price, supplier);

            mockItemRepository.Verify(r => r.GetItemByNameAsync(name), Times.Once);
            mockItemRepository.Verify(r => r.AddItemAsync(name, quantity, price, supplier), Times.Never);
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ShouldUpdateItem()
        {
            int itemId = 1;
            int newQuantity = 15;
            decimal newPrice = 150.0m;
            string newSupplier = "New Supplier";

            var existingItem = new Item { Id = itemId, Quantity = 10, Price = 100.0m, Supplier = "Test Supplier" };

            mockItemRepository.Setup(r => r.GetItemByIdAsync(itemId))
                               .ReturnsAsync(existingItem);

            await inventoryService.UpdateItemAsync(itemId, newQuantity, newPrice, newSupplier);

            mockItemRepository.Verify(r => r.GetItemByIdAsync(itemId), Times.Once);
            mockItemRepository.Verify(r => r.UpdateItemAsync(itemId, newQuantity, newPrice, newSupplier), Times.Once);
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateItemAsync_WithNonExistingItem_ShouldLogWarning()
        {
            int itemId = 1;
            int newQuantity = 15;
            decimal newPrice = 150.0m;
            string newSupplier = "New Supplier";

            mockItemRepository.Setup(r => r.GetItemByIdAsync(itemId))
                               .ReturnsAsync((Item)null);

            await inventoryService.UpdateItemAsync(itemId, newQuantity, newPrice, newSupplier);

            mockItemRepository.Verify(r => r.GetItemByIdAsync(itemId), Times.Once);
            mockItemRepository.Verify(r => r.UpdateItemAsync(itemId, newQuantity, newPrice, newSupplier), Times.Never);
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }
        [Fact]
        public async Task DeleteItemAsync_UserNotAdmin_ShouldNotDeleteItem()
        {
            var userId = "1";
            var user = new User { Id = userId };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new(ClaimTypes.NameIdentifier, userId)
            }));

            mockHttpContextAccessor.Setup(h => h.HttpContext.User).Returns(claimsPrincipal);
            mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
            mockUserManager.Setup(u => u.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);

            await inventoryService.DeleteItemAsync(1);

            mockLogger.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unauthorized attempt to delete item.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            mockItemRepository.Verify(r => r.DeleteItemAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteItemAsync_UserIsAdmin_ShouldDeleteItem()
        {
            var userId = "1";
            var user = new User { Id = userId };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new(ClaimTypes.NameIdentifier, userId)
            }));

            mockHttpContextAccessor.Setup(h => h.HttpContext.User).Returns(claimsPrincipal);
            mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
            mockUserManager.Setup(u => u.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);
            mockItemRepository.Setup(r => r.GetItemByIdAsync(It.IsAny<int>())).ReturnsAsync(new Item());

            await inventoryService.DeleteItemAsync(1);

            mockItemRepository.Verify(r => r.DeleteItemAsync(1), Times.Once);
            mockLogger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Item deleted successfully.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteItemAsync_ItemNotFound_ShouldLogWarning()
        {
            var userId = "1";
            var user = new User { Id = userId };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new(ClaimTypes.NameIdentifier, userId)
            }));

            mockHttpContextAccessor.Setup(h => h.HttpContext.User).Returns(claimsPrincipal);
            mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
            mockUserManager.Setup(u => u.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);
            mockItemRepository.Setup(r => r.GetItemByIdAsync(It.IsAny<int>())).ReturnsAsync((Item)null);

            await inventoryService.DeleteItemAsync(1);

            mockItemRepository.Verify(r => r.DeleteItemAsync(It.IsAny<int>()), Times.Never);
            mockLogger.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Item with ID 1 not found.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SearchItemsByName_WithValidName_ShouldReturnMatchingItems()
        {
            string itemName = "Test Item";
            var items = new List<Item>
            {
                new Item { Id = 1, Name = itemName + " 1", Quantity = 5, Price = 50.0m, Supplier = "Supplier A" },
                new Item { Id = 2, Name = "Another Item", Quantity = 10, Price = 100.0m, Supplier = "Supplier B" },
                new Item { Id = 3, Name = itemName + " 2", Quantity = 3, Price = 30.0m, Supplier = "Supplier A" }
            };

            mockItemRepository.Setup(r => r.SearchItemsByNameAsync(itemName))
                               .ReturnsAsync(items.Where(i => i.Name.Contains(itemName)));

            var result = await inventoryService.SearchItemsByName(itemName);

            Assert.Equal(2, result.Count());
            Assert.Contains(result, i => i.Name == itemName + " 1");
            Assert.Contains(result, i => i.Name == itemName + " 2");
        }

        [Fact]
        public async Task GenerateInventoryReportAsync_ShouldReturnCorrectReport()
        {
            var items = new List<Item>
        {
            new() { Id = 1, Name = "Item1", Quantity = 10, Price = 5.0m, Supplier = "Supplier1" },
            new() { Id = 2, Name = "Item2", Quantity = 20, Price = 10.0m, Supplier = "Supplier2" }
        };

            mockItemRepository.Setup(repo => repo.GetAllItemsAsync()).ReturnsAsync(items);

            var result = await inventoryService.GenerateInventoryReportAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var itemList = result.ToList();
            Assert.Equal(1, itemList[0].Id);
            Assert.Equal("Item1", itemList[0].Name);
            Assert.Equal(10, itemList[0].Quantity);
            Assert.Equal(5.0m, itemList[0].Price);
            Assert.Equal("Supplier1", itemList[0].Supplier);

            Assert.Equal(2, itemList[1].Id);
            Assert.Equal("Item2", itemList[1].Name);
            Assert.Equal(20, itemList[1].Quantity);
            Assert.Equal(10.0m, itemList[1].Price);
            Assert.Equal("Supplier2", itemList[1].Supplier);

            mockItemRepository.Verify(repo => repo.GetAllItemsAsync(), Times.Once);
        }
    }
}
