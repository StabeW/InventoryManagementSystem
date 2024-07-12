using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystemServices;
using InventoryManagementSystemData.Models;

namespace InventoryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddItem([FromBody] Item item)
        {
            await inventoryService.AddItemAsync(item.Name, item.Quantity, item.Price, item.Supplier);
            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] Item item)
        {
            await inventoryService.UpdateItemAsync(id, item.Quantity, item.Price, item.Supplier);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await inventoryService.DeleteItemAsync(id);
            return Ok();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchItems([FromQuery] string name)
        {
            var items = await inventoryService.SearchItemsByName(name);
            return Ok(items);
        }

        [HttpGet("report")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateInventoryReport()
        {
            var report = await inventoryService.GenerateInventoryReportAsync();
            return Ok(report);
        }
    }
}
