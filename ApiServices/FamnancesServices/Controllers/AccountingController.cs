using FamnancesServices.Business.Interfaces;
using FamnancesServices.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class AccountingController : ControllerBase
    {
        ITotalsByPeriodManager _totalsByPeriodManager { get; set; }

        public AccountingController(ITotalsByPeriodManager totalsByPeriodManager)
        {
            _totalsByPeriodManager = totalsByPeriodManager;
        }

        [HttpGet("CurentTotals")]
        public async Task<IActionResult> CurentTotals()
        {
            HttpContext.Items.TryGetValue("AccountId", out var accountId);
            return Ok(_totalsByPeriodManager.GetByCurrentPeriod(Guid.Parse(accountId.ToString())));
        }
    }
}
