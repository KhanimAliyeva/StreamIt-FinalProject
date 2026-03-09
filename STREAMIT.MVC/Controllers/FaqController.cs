using Microsoft.AspNetCore.Mvc;

namespace STREAMIT.MVC.Controllers
{
    public class FaqController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
