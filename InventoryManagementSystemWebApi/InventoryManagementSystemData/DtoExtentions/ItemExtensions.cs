using InventoryManagementSystemData.Models;
using InventoryManagementSystemDTOs.ItemsDto;

namespace InventoryManagementSystemData.DtoExtentions
{
    public static class ItemExtensions
    {
        public static ItemDto ToDto(this Item item)
        {
            if (item == null)
                return null;

            return new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Quantity = item.Quantity,
                Price = item.Price,
                Supplier = item.Supplier,
                CreatedOn = item.CreatedOn,
                DeletedOn = item.DeletedOn,
                ModifiedOn = item.ModifiedOn,
            };
        }
    }
}
