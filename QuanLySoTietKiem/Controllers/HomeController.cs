using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuanLySoTietKiem.Models;

namespace QuanLySoTietKiem.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                ViewBag.ThongBao = "You have not logged in to the system, please login to use";
                return View();
            }
            ViewBag.UserName = currentUser?.UserName;
            var checkRole = await _userManager.IsInRoleAsync(currentUser, "User");
            var checkRoleAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            if (checkRole)
            {
                return RedirectToAction("Index", "User");
            }
            else if (checkRoleAdmin)
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }
        [HttpGet]
        public ActionResult Privacy()
        {
            return View();
        }
    }
}
