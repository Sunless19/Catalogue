using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Models
{
    public class Class
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Information { get; set; }

        // Navigation property for Students
        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();

        // Foreign key for Teacher
        public int? TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher? Teacher { get; set; }
    }
}