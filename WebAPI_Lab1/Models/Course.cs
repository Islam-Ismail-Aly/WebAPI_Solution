using System.ComponentModel.DataAnnotations;

namespace WebAPI_Lab1.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(150)]
        public string? Description { get; set; }
        public int Duration { get; set; }
    }
}
