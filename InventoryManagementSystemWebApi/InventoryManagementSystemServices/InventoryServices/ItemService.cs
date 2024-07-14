using InventoryManagementSystemCommon;
using InventoryManagementSystemData.Repositories;
using InventoryManagementSystemDTOs.ItemsDto;

namespace InventoryManagementSystemServices.InventoryServices
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        public async Task<ServiceResult<ItemDto>> AddItemAsync(CreateItemDto createItemDto)
        {
            var item = await itemRepository.AddItemAsync(createItemDto);

            return ServiceResult<ItemDto>.Success(item);
        }

        public async Task<ServiceResult<string>> AddBulkItemsAsync(IEnumerable<CreateItemDto> createItemDtos)
        {
            await itemRepository.AddBulkItemsAsync(createItemDtos);

            return ServiceResult<string>.Success(ResponseMessageConstants.SuccessfullyAddedBulkItemsMessage);
        }

        public async Task<ServiceResult<ItemDto>> UpdateItemAsync(UpdateItemDto updateItemDto)
        {
            var item = await itemRepository.UpdateItemAsync(updateItemDto);

            if (item == null)
            {
                return ServiceResult<ItemDto>.NotFound(ResponseMessageConstants.ItemNotFoundErrorMessage);
            }

            return ServiceResult<ItemDto>.Success(item);
        }

        public async Task<ServiceResult<string>> DeleteItemAsync(int id)
        {
            var result = await itemRepository.DeleteItemAsync(id);

            if (!result)
            {
                return ServiceResult<string>.NotFound(ResponseMessageConstants.ItemNotFoundErrorMessage);
            }

            return ServiceResult<string>.Success(ResponseMessageConstants.SuccessfullyDeletedItemMessage);
        }

        public async Task<ServiceResult<IEnumerable<ItemDto>>> SearchItemsAsync(SearchFilterDto filterDto)
        {
            var items = await itemRepository.SearchItemsAsync(filterDto);

            return ServiceResult<IEnumerable<ItemDto>>.Success(items);
        }

        public async Task<ServiceResult<IEnumerable<ItemDto>>> GenerateInventoryReportAsync()
        {
            var items = await itemRepository.GetAllItemsAsync();

            return ServiceResult<IEnumerable<ItemDto>>.Success(items);
        }
    }
}