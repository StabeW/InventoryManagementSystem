using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using InventoryManagementSystemDTOs.ItemsDto;
using InventoryManagementSystemServices.InventoryServices;

namespace InventoryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/item")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService itemService;

        public ItemController(IItemService service)
        {
            itemService = service;
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddItemAsync([FromBody] CreateItemDto createItemDto)
        {
            var response = await itemService.AddItemAsync(createItemDto);

            return StatusCode((int)response.StatusCode, response.IsSuccess 
                ? response.Data 
                : new { error = response.ErrorMessage });
        }

        [HttpPost("add-bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBulkItemsAsync([FromBody] List<CreateItemDto> createItemDtos)
        {
            var response = await itemService.AddBulkItemsAsync(createItemDtos);

            return StatusCode((int)response.StatusCode, response.IsSuccess 
                ? response.Data 
                : new { error = response.ErrorMessage });
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateItemAsync([FromBody] UpdateItemDto updateItemDto)
        {
            var response = await itemService.UpdateItemAsync(updateItemDto);

            return StatusCode((int)response.StatusCode, response.IsSuccess 
                ? response.Data 
                : new { error = response.ErrorMessage });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteItemAsync(int id)
        {
            var response = await itemService.DeleteItemAsync(id);

            return StatusCode((int)response.StatusCode, response.IsSuccess 
                ? response.Data 
                : new { error = response.ErrorMessage });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchItemsAsync([FromQuery] SearchFilterDto filterDto)
        {
            var response = await itemService.SearchItemsAsync(filterDto);

            return StatusCode((int)response.StatusCode, response.IsSuccess 
                ? response.Data 
                : new { error = response.ErrorMessage });
        }

        [HttpGet("report")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateInventoryReportAsync()
        {
            var response = await itemService.GenerateInventoryReportAsync();

            return StatusCode((int)response.StatusCode, response.IsSuccess 
                ? response.Data 
                : new { error = response.ErrorMessage });
        }
    }
}
