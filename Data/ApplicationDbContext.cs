using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Reflection.Emit;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Workspaces> Workspaces { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Statuses> Status { get; set; }
        public DbSet<Working> Workings { get; set; }
        public DbSet<WorkspaceRoles> WorkspaceRoles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AssignedTo> AssignedTos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<Tasks>()
            //    .HasKey(t => new { t.WorkspaceID, t.StatusID });

            builder.Entity<Tasks>()
                .HasOne(t => t.Status)
                .WithMany(s => s.Tasks)
                .HasForeignKey(t => t.StatusID);

            builder.Entity<Tasks>()
                .HasOne(t => t.Workspace)
                .WithMany(w => w.Tasks)
                .HasForeignKey(t => t.WorkspaceID);

            builder.Entity<Working>()
                .HasOne<ApplicationUser>()
                .WithMany(u => u.Workings)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Working>()
                .HasOne(w => w.Workspace)
                .WithMany(ws => ws.Workings)
                .HasForeignKey(w => w.WorkspaceID);

            builder.Entity<Working>()
                .HasOne(w => w.WorkspaceRole)
                .WithMany(r => r.Workings)
                .HasForeignKey(w => w.WorkspaceRoleID);

            builder.Entity<AssignedTo>()
                .HasKey(at => new { at.userID, at.taskID });

            builder.Entity<AssignedTo>()
                .HasOne(at => at.User)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(at => at.userID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AssignedTo>()
                .HasOne(at => at.Tasks)
                .WithMany(t => t.AssignedUsers)
                .HasForeignKey(at => at.taskID);

            builder.Entity<Comment>()
                .HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskID);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.ID);

                entity.HasOne(c => c.Task)
                      .WithMany(t => t.Comments)
                      .HasForeignKey(c => c.TaskID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.Content)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(c => c.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");
            });



            builder.Entity<WorkspaceRoles>().HasData(
                new WorkspaceRoles { Id = 1, Name = "Organizator" },
                new WorkspaceRoles { Id = 2, Name = "Membru" }
            );

        }
    }
}
