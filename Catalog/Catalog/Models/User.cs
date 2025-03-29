using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int UserId { get; set; }

        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }
    }
}
