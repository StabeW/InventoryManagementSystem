namespace InventoryManagementSystemDTOs.ItemsDto
{
    public class UpdateItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }

        public string Supplier { get; set; }
    }
}
