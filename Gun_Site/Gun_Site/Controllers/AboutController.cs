using Microsoft.AspNetCore.Mvc;

namespace Gun_Site.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
