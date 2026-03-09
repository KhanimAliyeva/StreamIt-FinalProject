using Microsoft.AspNetCore.Mvc;

namespace STREAMIT.MVC.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
