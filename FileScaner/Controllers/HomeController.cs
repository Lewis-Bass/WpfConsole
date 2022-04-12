using Microsoft.AspNetCore.Mvc;

namespace FileScaner.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("I AM HERE");
            //return View();
        }
    }
}
