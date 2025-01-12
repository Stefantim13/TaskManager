using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class WorkspaceRoles
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public virtual Working? Working { get; set; }
    }
}
