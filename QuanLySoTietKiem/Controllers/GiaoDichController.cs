using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Entity;

namespace QuanLySoTietKiem.Controllers;

public class GiaoDichController : Controller
{

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GiaoDichController> _logger;
    //Cho phép dùng constructor để inject các service vào controller
    [ActivatorUtilitiesConstructor]
    public GiaoDichController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<GiaoDichController> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }
    [HttpGet]
    [Authorize(Roles = RoleConstants.User)]
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }
        //Lấy danh sách sổ tiết kiệm của người dùng 
        var savingsAccounts = await _context.SoTietKiems
        .Where(s => s.UserId == currentUser.Id)
        .Select(s => new SelectListItem
        {
            Value = s.MaSoTietKiem.ToString(),
            Text = s.Code
        })
        .ToListAsync();

        ViewBag.SavingsAccounts = new SelectList(savingsAccounts, "Value", "Text");
        return View();
    }


    [HttpPost]
    [Authorize(Roles = RoleConstants.User)]
    public async Task<IActionResult> Index(int selectedAccountId)
    {
        var transactions = await _context.GiaoDichs
            .Where(g => g.MaSoTietKiem == selectedAccountId)
            .ToListAsync();

        // Thay đổi cách logging
        _logger.LogInformation("Found {Count} transactions for account {AccountId}",
            transactions.Count, selectedAccountId);

        ViewBag.Transactions = transactions;

        // Fetch the user's savings accounts again for the dropdown
        var currentUser = await _userManager.GetUserAsync(User);
        var savingsAccounts = await _context.SoTietKiems
            .Where(s => s.UserId == currentUser.Id)
            .Select(s => new { s.Code, s.MaSoTietKiem })
            .ToListAsync();

        ViewBag.SavingsAccounts = new SelectList(savingsAccounts, "MaSoTietKiem", "Code");
        return View();
    }
}