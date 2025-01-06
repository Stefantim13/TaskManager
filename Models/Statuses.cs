using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class Statuses
    {
        [Key]

        public int ID { get; set; }

        public string? Color { get; set; }

        public string? Name { get; set; }

        public virtual ICollection<Tasks>? Tasks { get; set; } = new List<Tasks>();
    }
}
