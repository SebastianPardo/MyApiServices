using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    [Route("Api/[controller]")]
    public class PeriodsController : ControllerBase
    {
        IPeriodManager _periodManager;
        public PeriodsController(IPeriodManager periodManager)
        {
            _periodManager = periodManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetPeriods()
        {
            var periods = _periodManager.GetAll();
            return Ok(periods);
        }
    }
}
