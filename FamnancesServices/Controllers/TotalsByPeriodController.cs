using Famnances.AuthMiddleware;
using Famnances.DataCore.Entities;
using FamnancesServices.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TotalsByPeriodController : ControllerBase
    {
        TotalsByPeriodManager _totalsByPeriodManager;

        public TotalsByPeriodController(TotalsByPeriodManager totalsByPeriodManager)
        {
            _totalsByPeriodManager = totalsByPeriodManager;
        }

        [HttpGet("GetCurrentPeriod")]
        public async Task<IActionResult> GetCurrentPeriod()
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            TotalsByPeriod? totalsByPeriod = _totalsByPeriodManager.GetByCurrentPeriod(Guid.Parse(accountId.ToString()));
            return Ok(totalsByPeriod);

        }
    }
}
