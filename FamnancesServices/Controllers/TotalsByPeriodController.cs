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

        [HttpGet("GetLastPeriod")]
        public async Task<ActionResult<PeriodDates>> GetLastPeriod()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            PeriodDates PeriodDates = _totalsByPeriodManager.GetMostRecent(Guid.Parse(accountId.ToString()));
            return Ok(PeriodDates);
        }

        [HttpGet("GetCurrentPeriod")]
        public async Task<ActionResult<PeriodDates?>> GetCurrentPeriod()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            PeriodDates? PeriodDates = _totalsByPeriodManager.GetByCurrentDay(Guid.Parse(accountId.ToString()));
            return Ok(PeriodDates);

        }

        [HttpGet("GetByDate/{date}")]
        public async Task<ActionResult<PeriodDates?>> GetByDate(DateTime date)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            PeriodDates? PeriodDates = _totalsByPeriodManager.GetByDate(Guid.Parse(accountId.ToString()), date);
            return Ok(PeriodDates);

        }
    }
}
