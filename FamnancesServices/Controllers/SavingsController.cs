using Famnances.AuthMiddleware;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class SavingsController : Controller
    {
        ISavingRecordManager _savingRecordManager;
        ISavingsPocketManager _avingsPocketManager;
        public SavingsController() { }
    }
}
