using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Authorize]
    public class WorkspaceController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public WorkspaceController(
            ApplicationDbContext _db,
            UserManager<ApplicationUser> _userManager,
            RoleManager<IdentityRole> _roleManager
        ){
            db = _db;
            userManager = _userManager;
            roleManager = _roleManager;
        }

        public async Task<IActionResult> MyWorkspaces(int? id)
        {
            List<Workspaces?> workspaces = new List<Workspaces?>();
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return Forbid();
            }

            bool isAdmin = await userManager.IsInRoleAsync(user, "Administrator");

            if (isAdmin)
            {
                ViewBag.isAdmin = true;
                workspaces = await db.Workspaces.ToListAsync();
            }
            else
            {
                workspaces = await db.Workings
                    .Where(w => w.UserId == user.Id)
                    .Include(w => w.Workspace)
                    .Select(w => w.Workspace)
                    .ToListAsync();
            }

            return View(workspaces);
        }

        public async Task<IActionResult> ShowWorkspace(int id)
        {
            var workspace = await db.Workspaces
                .Include(w => w.Tasks)
                .FirstOrDefaultAsync(w => w.ID == id);

            if (workspace == null)
            {
                return NotFound("Workspace not found.");
            }

            var user = await userManager.GetUserAsync(User);
            if(user == null)
            {
                return Forbid();
            }
            var admin = await userManager.IsInRoleAsync(user, "Administrator");
            if(admin)
            {
                ViewBag.isAdmin = true;
            }
            var working = await db.Workings.Where(w => w.WorkspaceID == id && w.UserId == user.Id).FirstOrDefaultAsync();
            if(working != null && working.WorkspaceRoleID == 1)
            {
                ViewBag.isOrg = true;
            }

            ViewBag.WorkspaceId = id;
            ViewBag.WorkspaceName = workspace.Name;
            return View(workspace.Tasks);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWorkspace(int WorkspaceId)
        {
            var workspace = await db.Workspaces
                .Include(w => w.Tasks)
                .Include(w => w.Workings)
                .FirstOrDefaultAsync(w => w.ID == WorkspaceId);

            if (workspace == null)
            {
                return NotFound("Workspace not found.");
            }

            if (workspace.Tasks.Any())
            {
                db.Tasks.RemoveRange(workspace.Tasks);
            }

            if (workspace.Workings.Any())
            {
                db.Workings.RemoveRange(workspace.Workings);
            }


            db.Workspaces.Remove(workspace);
            db.SaveChanges();

            return RedirectToAction("MyWorkspaces", "Workspace");
        }

        [HttpGet]
        public IActionResult CreateWorkspace()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddMember(int workspaceID)
        {
            var user = await userManager.GetUserAsync(User);
            var userID = user.Id;
            var org = db.Workings.FirstOrDefaultAsync(w => w.WorkspaceID == workspaceID && w.UserId == userID && w.WorkspaceRoleID == 1);

            if (org == null)
            {
                return Forbid();
            }

            ViewBag.WorkspaceID = workspaceID;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddMember(int workspaceID, string email)
        {

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Forbid();
            }

            var working = new Working
            {
                UserId = await userManager.GetUserIdAsync(user),
                WorkspaceID = workspaceID,
                WorkspaceRoleID = 2
            };

            db.Workings.Add(working);
            db.SaveChanges();



            return RedirectToAction("ShowWorkspace", "Workspace", new { id = workspaceID });
        }


        [HttpPost]
        public async Task<IActionResult> CreateWorkspace(Workspaces model)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return Forbid();
            }

            db.Workspaces.Add(model);
            db.SaveChanges();
            
            var organizator = await db.WorkspaceRoles.FindAsync(1);
            if(organizator == null)
            {
                var org = new WorkspaceRoles
                {
                    Id = 1,
                    Name = "Organizator",
                };
                db.WorkspaceRoles.Add(org);
                await db.SaveChangesAsync();
            }




            var membru = await db.WorkspaceRoles.FindAsync(2);
            if (membru == null)
            {
                var mem = new WorkspaceRoles
                {
                    Id = 2,
                    Name = "Membru",
                };
                db.WorkspaceRoles.Add(mem);
                await db.SaveChangesAsync();
            }

            var working = new Working
            {
                UserId = userManager.GetUserId(User),
                WorkspaceID = model.ID,
                WorkspaceRoleID = 1
            };

            db.Workings.Add(working);
            db.SaveChanges();

            return Redirect("SuccesPage");
        }

        public IActionResult SuccesPage()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
