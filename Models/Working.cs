using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class Working
    {
        [Key]
        public int Id { get; set; } 
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? WorkspaceID { get; set; }

        public virtual Workspaces? Workspace { get; set; }

        public int? WorkspaceRoleID { get; set; }
        [ForeignKey("WorkspaceRoleID")]
        public virtual WorkspaceRoles? WorkspaceRole { get; set; }
    }
}
