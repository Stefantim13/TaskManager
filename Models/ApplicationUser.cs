using Microsoft.AspNetCore.Identity;


namespace TaskManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Working>? Workings { get; set; } = new List<Working>();
        public virtual ICollection<Tasks>? Tasks { get; set; } = new List<Tasks>();

        public virtual ICollection<AssignedTo>? AssignedTasks { get; set; } = new List<AssignedTo>();
        public virtual ICollection<Comment>? Comments { get; set; } = new List<Comment>();
    }
}
