using InventoryManagementSystemData.Common;

using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystemData.Models
{
    public class Item : BaseDeletableModel<int>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Supplier { get; set; }
    }
}
