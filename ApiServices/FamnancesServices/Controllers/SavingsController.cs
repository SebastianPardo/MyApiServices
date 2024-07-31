using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    public class SavingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
