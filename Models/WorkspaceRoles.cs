using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class WorkspaceRoles
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<Working?> Workings { get; set; } = new List<Working?>();
    }
}
