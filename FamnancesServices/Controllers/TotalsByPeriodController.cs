using Famnances.Core.Security;
using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
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
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            TotalsByPeriod? totalsByPeriod = _totalsByPeriodManager.GetByCurrentDay(Guid.Parse(accountId.ToString()));
            return Ok(totalsByPeriod);

        }

        [HttpGet("GetByDate/{date}")]
        public async Task<ActionResult<TotalsByPeriod?>> GetByDate(DateTime date)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            TotalsByPeriod? totalsByPeriod = _totalsByPeriodManager.GetByDate(Guid.Parse(accountId.ToString()), date);
            return Ok(totalsByPeriod);

        }
    }
}
