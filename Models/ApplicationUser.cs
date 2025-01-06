using Microsoft.AspNetCore.Identity;


namespace TaskManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Working>? Workings { get; set; } = new List<Working>();
        public virtual ICollection<Tasks>? Tasks { get; set; } = new List<Tasks>();
    }
}
