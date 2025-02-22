using Microsoft.AspNetCore.Mvc;

namespace Controllers;
public class HuongDanController : Controller
{
  public IActionResult Index()
  {
    return View();
  }
}