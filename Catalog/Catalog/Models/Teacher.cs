using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Models
{
    public class Teacher : User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
