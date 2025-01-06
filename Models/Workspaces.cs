using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class Workspaces
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "The name of the workspace is required!")]
        public string? Name { get; set; }

        public virtual ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
        public virtual ICollection<Working> Workings { get; set; } = new List<Working>();



    }
}
