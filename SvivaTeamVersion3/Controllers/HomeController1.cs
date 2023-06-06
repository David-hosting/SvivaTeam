using Microsoft.AspNetCore.Mvc;

namespace SvivaTeamVersion3.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
