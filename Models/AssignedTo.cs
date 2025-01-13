namespace TaskManager.Models
{
    public class AssignedTo
    {
        public string? userID { get; set; }
        public int taskID { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Tasks? Tasks { get; set; }
    }
}