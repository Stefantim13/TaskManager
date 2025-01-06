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
                .HasForeignKey(w => w.UserId);

            builder.Entity<Working>()
                .HasOne(w => w.Workspace)
                .WithMany(ws => ws.Workings)
                .HasForeignKey(w => w.WorkspaceID);

            builder.Entity<Working>()
                .HasOne(w => w.WorkspaceRole)
                .WithOne(r => r.Working)
                .HasForeignKey <Working> (w => w.WorkspaceRoleID);



            builder.Entity<WorkspaceRoles>().HasData(
                new WorkspaceRoles { Id = 1, Name = "Organizator" },
                new WorkspaceRoles { Id = 2, Name = "Membru" }
            );

        }
    }
}
