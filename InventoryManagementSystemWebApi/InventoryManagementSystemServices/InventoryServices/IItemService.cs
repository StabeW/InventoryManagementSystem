using InventoryManagementSystemDTOs.ItemsDto;

namespace InventoryManagementSystemServices.InventoryServices
{
    public interface IItemService
    {
        Task<ServiceResult<ItemDto>> AddItemAsync(CreateItemDto createItemDto);

        Task<ServiceResult<string>> AddBulkItemsAsync(IEnumerable<CreateItemDto> createItemDtos);

        Task<ServiceResult<ItemDto>> UpdateItemAsync(UpdateItemDto updateItemDto);

        Task<ServiceResult<string>> DeleteItemAsync(int id);

        Task<ServiceResult<IEnumerable<ItemDto>>> SearchItemsAsync(SearchFilterDto criteriaDto);

        Task<ServiceResult<IEnumerable<ItemDto>>> GenerateInventoryReportAsync();
    }
}
