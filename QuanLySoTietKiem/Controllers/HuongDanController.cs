using Microsoft.AspNetCore.Mvc;

namespace QuanLySoTietKiem.Controllers;
public class HuongDanController : Controller
{
  public IActionResult Index()
  {
    return View();
  }
}