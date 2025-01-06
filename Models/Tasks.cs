using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TaskManager.Models
{
    
    public class Tasks
    {
        [Key]

        public int ID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? StatusID { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        public string? Title { get; set; }

        public virtual Statuses? Status { get; set; }

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? WorkspaceID { get; set; }

        public virtual Workspaces? Workspace { get; set; }
    }
}
