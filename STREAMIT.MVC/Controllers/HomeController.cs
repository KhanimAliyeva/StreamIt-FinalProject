using Microsoft.AspNetCore.Mvc;
using STREAMIT.MVC.Models;
using System.Diagnostics;

namespace STREAMIT.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
