using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystemData.Common
{
    public interface IBaseEntity<TKey> : IAuditInfo
    {
        [Key]
        TKey Id { get; set; }

        bool IsDeleted { get; set; }

        DateTime? DeletedOn { get; set; }
    }
}
