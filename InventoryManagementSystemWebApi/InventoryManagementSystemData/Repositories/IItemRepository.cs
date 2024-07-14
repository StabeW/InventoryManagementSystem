using InventoryManagementSystemDTOs.ItemsDto;

namespace InventoryManagementSystemData.Repositories
{
    public interface IItemRepository
    {
        Task<ItemDto> AddItemAsync(CreateItemDto createItemDto);

        Task AddBulkItemsAsync(IEnumerable<CreateItemDto> createItemDtos);

        Task<ItemDto> UpdateItemAsync(UpdateItemDto updateItemDto);

        Task<bool> DeleteItemAsync(int id);

        Task<IEnumerable<ItemDto>> SearchItemsAsync(SearchFilterDto criteriaDto);

        Task<IEnumerable<ItemDto>> GetAllItemsAsync();
    }
}
