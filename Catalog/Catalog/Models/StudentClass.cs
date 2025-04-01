using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Models
{
    public class StudentClass
    {
        [Key]
        public int StudentId { get; set; }

        [Key]
        public int ClassId { get; set; }

        // Foreign key for Student
        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        // Foreign key for Class
        [ForeignKey("ClassId")]
        public Class Class { get; set; }
    }
}