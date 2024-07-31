using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    public class InflowsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
