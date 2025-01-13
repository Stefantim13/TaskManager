using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TaskManager.Models
{
    
    public class Tasks
    {
        [Key]

        public int ID { get; set; }

        public int? StatusID { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        public string? Title { get; set; }

        public virtual Statuses? Status { get; set; }

        public string? Description { get; set; }

        public DateTime? DataStart { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }

        public int? WorkspaceID { get; set; }

        public virtual Workspaces? Workspace { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<AssignedTo>? AssignedUsers { get; set; } = new List<AssignedTo>();
    }
}
