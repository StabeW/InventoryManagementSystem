using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystemData.Common
{
    public abstract class BaseModel<TKey>
    {
        [Key]
        public TKey Id { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; }
    }
}
