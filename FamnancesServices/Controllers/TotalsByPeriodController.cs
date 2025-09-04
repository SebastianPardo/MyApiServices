using Famnances.AuthMiddleware;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [Route("Api/[controller]")]
    [ApiController]
    public class TotalsByPeriodController : ControllerBase
    {
        ITotalsByPeriodManager _totalsByPeriodManager;

        public TotalsByPeriodController(ITotalsByPeriodManager totalsByPeriodManager)
        {
            _totalsByPeriodManager = totalsByPeriodManager;
        }

        [HttpGet("GetCurrentPeriod")]
        public async Task<ActionResult<TotalsByPeriod?>> GetCurrentPeriod()
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            TotalsByPeriod? totalsByPeriod = _totalsByPeriodManager.GetByCurrentPeriod(Guid.Parse(accountId.ToString()));
            return Ok(totalsByPeriod);

        }
    }
}
