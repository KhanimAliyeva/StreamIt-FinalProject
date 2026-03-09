using Microsoft.AspNetCore.Mvc;

namespace STREAMIT.MVC.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
