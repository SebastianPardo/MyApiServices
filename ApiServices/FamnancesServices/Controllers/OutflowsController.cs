using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    public class OutflowsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
