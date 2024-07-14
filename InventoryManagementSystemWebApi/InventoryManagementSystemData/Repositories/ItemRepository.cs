using InventoryManagementSystemData.DtoExtentions;
using InventoryManagementSystemData.Models;
using InventoryManagementSystemDTOs.ItemsDto;

using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

namespace InventoryManagementSystemData.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly InventoryManagementSystemDbContext context;

        public ItemRepository(InventoryManagementSystemDbContext context)
        {
            this.context = context;
        }

        public async Task<ItemDto> AddItemAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Quantity = createItemDto.Quantity,
                Price = createItemDto.Price,
                Supplier = createItemDto.Supplier
            };

            context.Items.Add(item);
            await context.SaveChangesAsync();

            return item.ToDto();
        }

        public async Task AddBulkItemsAsync(IEnumerable<CreateItemDto> createItemDtos)
        {
            var items = createItemDtos
                .Select(dto => new Item
                {
                    Name = dto.Name,
                    Quantity = dto.Quantity,
                    Price = dto.Price,
                    Supplier = dto.Supplier
                })
                .ToList();

            await context.Items.AddRangeAsync(items);
            await context.SaveChangesAsync();
        }

        public async Task<ItemDto> UpdateItemAsync(UpdateItemDto updateItemDto)
        {
            var item = await context.Items.FindAsync(updateItemDto.Id);

            if (item == null)
                return null;

            UpdateItemProperties(item, updateItemDto);

            await context.SaveChangesAsync();
            return item.ToDto();
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await context.Items.FindAsync(id);

            if (item == null)
                return false;

            item.IsDeleted = true;
            item.DeletedOn = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ItemDto>> SearchItemsAsync(SearchFilterDto filterDto)
        {
            var filter = BuildExpressionFilter(filterDto);

            return await context
                .Items
                .Where(filter)
                .Select(item => item.ToDto())
                .ToListAsync();
        }

        public async Task<IEnumerable<ItemDto>> GetAllItemsAsync() =>
            await context
                .Items
                .Where(item => !item.IsDeleted)
                .Select(item => item.ToDto())
                .ToListAsync();

        private static void UpdateItemProperties(Item item, UpdateItemDto updateItemDto)
        {
            bool isModified = false;

            if (updateItemDto.Quantity.HasValue && item.Quantity != updateItemDto.Quantity.Value)
            {
                item.Quantity = updateItemDto.Quantity.Value;
                isModified = true;
            }

            if (updateItemDto.Price.HasValue && item.Price != updateItemDto.Price.Value)
            {
                item.Price = updateItemDto.Price.Value;
                isModified = true;
            }

            if (!string.IsNullOrEmpty(updateItemDto.Supplier) && item.Supplier != updateItemDto.Supplier)
            {
                item.Supplier = updateItemDto.Supplier;
                isModified = true;
            }

            if (!string.IsNullOrEmpty(updateItemDto.Name) && item.Name != updateItemDto.Name)
            {
                item.Name = updateItemDto.Name;
                isModified = true;
            }

            if (isModified)
            {
                item.ModifiedOn = DateTime.UtcNow;
            }
        }

        private static Expression<Func<Item, bool>> BuildExpressionFilter(SearchFilterDto filterDto)
        {
            return item =>
                (string.IsNullOrEmpty(filterDto.Name) || item.Name.Contains(filterDto.Name)) &&
                (!filterDto.Id.HasValue || item.Id == filterDto.Id.Value) &&
                (!filterDto.Quantity.HasValue || item.Quantity == filterDto.Quantity.Value) &&
                (!filterDto.Price.HasValue || item.Price == filterDto.Price.Value) &&
                (string.IsNullOrEmpty(filterDto.Supplier) || item.Supplier.Contains(filterDto.Supplier)) &&
                (!filterDto.ModifiedOn.HasValue || item.ModifiedOn >= filterDto.ModifiedOn.Value) &&
                (!filterDto.CreatedOn.HasValue || item.CreatedOn >= filterDto.CreatedOn.Value) &&
                (!filterDto.DeletedOn.HasValue || item.DeletedOn >= filterDto.DeletedOn.Value) &&
                !item.IsDeleted;
        }
    }
}
