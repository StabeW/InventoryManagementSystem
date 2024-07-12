using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystemData.Models
{
    public class User : IdentityUser
    {
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public override string UserName { get; set; }

        public override string Email { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }
    }
}
