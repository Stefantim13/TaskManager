using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext _db, UserManager<ApplicationUser> _userManager)
        {
            db = _db;
            userManager = _userManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Workspaces()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("AdminPanel");
            }

            var userWorkings = db.Workings.Where(w => w.UserId == userId);
            db.Workings.RemoveRange(userWorkings);

            var userComments = db.Comments.Where(c => c.UserId == userId);
            db.Comments.RemoveRange(userComments);

            var userAssignments = db.AssignedTos.Where(a => a.userID == userId);
            db.AssignedTos.RemoveRange(userAssignments);

            await db.SaveChangesAsync();

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return RedirectToAction("AdminPanel");
            }

            return RedirectToAction("AdminPanel");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminPanel()
        {
            var users = await userManager.Users.ToListAsync();
            return View(users);
        }

    }
}
