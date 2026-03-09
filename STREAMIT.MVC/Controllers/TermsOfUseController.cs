using Microsoft.AspNetCore.Mvc;

namespace STREAMIT.MVC.Controllers
{
    public class TermsOfUseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
