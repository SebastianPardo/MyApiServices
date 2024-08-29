using FamnancesServices.Business.Interfaces;
using FamnancesServices.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class ManagementController : ControllerBase
    {
        IPeriodManager _periodManager;
        public ManagementController(IPeriodManager periodManager)
        {
            _periodManager = periodManager;
        }

        [HttpGet("GetPeriods")]
        public async Task<IActionResult> GetPeriods()
        {
            var periods = _periodManager.GetAll();
            return Ok(periods);
        }
    }
}
