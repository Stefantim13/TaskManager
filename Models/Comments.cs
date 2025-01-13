using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class Comment
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int TaskID { get; set; }

        [ForeignKey("TaskID")]
        public virtual Tasks Task { get; set; }

        [Required(ErrorMessage = "The comment content is required.")]
        [StringLength(500, ErrorMessage = "The comment content cannot exceed 500 characters.")]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

}
