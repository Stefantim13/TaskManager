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
        public IActionResult CreateTask(int? id)
        {
            ViewBag.WorkspaceID = id;
            return View();
        }

        [HttpPost]
        public IActionResult CreateTask(Tasks model)
        {
            var status = new Statuses { Name = "In Progress" };
            db.Status.Add(status);
            db.SaveChanges();
            // is logged in if not return;
            db.Tasks.Add(model);
            db.SaveChanges();
            return RedirectToAction("MyTasks", "Tasks", new { id = model.WorkspaceID });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
