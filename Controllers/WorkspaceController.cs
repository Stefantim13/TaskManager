using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
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
            List<Workspaces> workspaces = new List<Workspaces>();
            var user = await userManager.GetUserAsync(User);
            ///if (user == null)
            /// return Unauthorized();


            ///var roles = await userManager.GetRolesAsync(user);
            bool isAdmin = true;/// roles.Contains("Administrator");

            if(isAdmin)
            {
                workspaces = await db.Workspaces
                    .Include(w => w.Tasks)
                    .Where(w => w.Workings.UserId == user.Id)
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

            return View(workspaces);
        }

        [HttpGet]
        public IActionResult CreateWorkspace()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateWorkspace(Workspaces model)
        {
            // is logged in if not return;
            db.Workspaces.Add(model);
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
