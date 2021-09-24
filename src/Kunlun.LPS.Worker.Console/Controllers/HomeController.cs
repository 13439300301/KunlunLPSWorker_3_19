using Kunlun.LPS.Worker.Core;
using Microsoft.AspNetCore.Mvc;

namespace Kunlun.LPS.Worker.Console.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Version = LPSHelper.GetProductVersion();
            return View();
        }
    }
}