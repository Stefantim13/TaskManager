using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        public TasksController(ApplicationDbContext _db, UserManager<ApplicationUser> _userManager)
        {
            db = _db;
            userManager = _userManager;
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int TaskID, int StatusID)
        {
            var task = await db.Tasks.FindAsync(TaskID);
            if (task == null)
            {
                TempData["Error"] = "Task not found.";
                return RedirectToAction("TaskDetails", new { id = TaskID });
            }

            var status = await db.Status.FindAsync(StatusID);
            if (status == null)
            {
                TempData["Error"] = "Invalid status selected.";
                return RedirectToAction("TaskDetails", new { id = TaskID });
            }

            task.StatusID = StatusID;
            db.Tasks.Update(task);
            await db.SaveChangesAsync();

            TempData["Success"] = "Task status updated successfully!";
            return RedirectToAction("TaskDetails", "Tasks", new { id = TaskID });
        }


        [HttpGet]
        public async Task<IActionResult> TaskDetails(int? id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Forbid();
            }

            var task = await db.Tasks
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Include(t => t.Status)
                .Include(t => t.Workspace)
                .Include(t => t.AssignedUsers)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(t => t.ID == id);
                

            if (task == null)
            {
                return NotFound();
            }

            ViewBag.Statuses = await db.Status.ToListAsync();

            return View(task);
        }


        public async Task<IActionResult> MyTasks(int? id)
        {
            ViewBag.WorkspaceName = null;
            List<Tasks> tasks = new List<Tasks>();
            if (id != null)
            {
                tasks = await db.Tasks
                    .Include(w => w.Workspace)
                    .Include(s => s.Status)
                    .Where(t => t.WorkspaceID == id)
                    .ToListAsync();
                ViewBag.WorkspaceName = db.Workspaces.Include(w => w.ID == id).First().Name;
                return View(tasks);
            }
            ///var user = await userManager.GetUserAsync(User);
            ///if (user == null)
            /// return Unauthorized();


            ///var roles = await userManager.GetRolesAsync(user);
            bool isAdmin = true;/// roles.Contains("Administrator");

            if (isAdmin)
            {
                tasks = await db.Tasks
                    .Include(w => w.Workspace)
                    .Include(s => s.Status)
                    .ToListAsync();
            }
            else
            {
                return View();
                ///workspaces = await db.Workings
                //    .Where(w => w.UserId == user.Id)
                //    .Include(w => w.Workspace)
                //    .Select(w => w.Workspace)
                //  .ToListAsync();
            }


            return View(tasks);

        }

        [HttpGet]
        public async Task<IActionResult> CreateTask(int? workspaceID)
        {
            var user = await userManager.GetUserAsync(User);
            var userID = user.Id;
            var org = db.Workings.FirstOrDefaultAsync(w => w.WorkspaceID == workspaceID && w.UserId == userID && w.WorkspaceRoleID == 1);

            if(org == null)
            {
                return Forbid();
            }
            ViewBag.WorkspaceID = workspaceID;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(Tasks model)
        {
            var user = await userManager.GetUserAsync(User);
            if(user == null || model.DueDate < DateTime.Now)
            {
                return Forbid();
            }

            /*
            var statuses = new List<Statuses>
            {
                new Statuses { Name = "Not Started", Color = "red" },
                new Statuses { Name = "In Progress", Color = "yellow" },
                new Statuses { Name = "Completed", Color = "green" }
            };
            db.Status.AddRange(statuses);
            db.SaveChanges();
            */

            db.Tasks.Add(model);
            db.SaveChanges();
            return RedirectToAction("ShowWorkspace", "Workspace", new { id = model.WorkspaceID });
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(int taskId, string content, string userId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Comment content cannot be empty.";
                return RedirectToAction("TaskDetails", new { id = taskId });
            }

            var task = await db.Tasks.FirstOrDefaultAsync(t => t.ID == taskId);
            if (task == null)
            {
                TempData["Error"] = "Task not found.";
                return RedirectToAction("MyTasks");
            }

            var comment = new Comment
            {
                TaskID = taskId,
                Content = content,
                UserId = userId,
            };

            db.Comments.Add(comment);
            await db.SaveChangesAsync();

            TempData["Success"] = "Comment added successfully.";
            return RedirectToAction("TaskDetails", new { id = taskId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentID)
        {
            var comment = await db.Comments.FindAsync(commentID);

            if (comment == null)
            {
                return Forbid();
            }

            db.Remove(comment);
            db.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateComment(int commentID, string newDescription)
        {
            var comment = await db.Comments.FindAsync(commentID);

            if (comment == null)
            {
                return Forbid();
            }

            comment.Content = newDescription;

            db.Entry(comment).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return NoContent();
        }


        [HttpPost]
        public async Task<IActionResult> DeleteTask(int taskID, int WorkspaceID)
        {
            var task = await db.Tasks.FindAsync(taskID);

            if (task == null)
            {
                return Forbid();
            }

            var user = await userManager.GetUserAsync(User);
            var userID = user.Id;
            var org = db.Workings.FirstOrDefaultAsync(w => w.WorkspaceID == WorkspaceID && w.UserId == userID && w.WorkspaceRoleID == 1);

            if (org == null)
            {
                return Forbid();
            }

            db.Tasks.Remove(task);
            db.SaveChanges();

            return RedirectToAction("ShowWorkspace", "Workspace", new {id = WorkspaceID});
        }

        [HttpPost]
        public async Task<IActionResult> AssignTask(int TaskID, string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Forbid();
            }

            var assign = new AssignedTo
            {
                userID = user.Id,
                taskID = TaskID
            };

            try
            {
                db.AssignedTos.Add(assign);
                await db.SaveChangesAsync();
            } catch(Exception e)
            {
                Console.WriteLine(e);
            }

            return NoContent();
        }
    }


}
