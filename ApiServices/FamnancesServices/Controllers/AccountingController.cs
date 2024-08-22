using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
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
        public IActionResult CurentTotals()
        {
            HttpContext.Items.TryGetValue("AccountId", out var accountId);
            TotalsByPeriod? totalsByPeriod = _totalsByPeriodManager.GetByCurrentPeriod(Guid.Parse(accountId.ToString()));
            if (totalsByPeriod != null)
            {
                decimal balance = totalsByPeriod.TotalIncomes - totalsByPeriod.TotalExpenses;
                SummaryModel summaryModel = new SummaryModel
                {
                    PeriodBalance = (int)balance,
                    PeriodCentsBalance = (int)(balance - (int)balance),
                    PeriodSavings = totalsByPeriod.TotalSavings,
                    PeriodSavingsExpeses = totalsByPeriod.TotalSavingsExpenses,
                    FullBudget = totalsByPeriod.User.TotalBudget,
                    TotalSavings = totalsByPeriod.User.TotalSavings,
                    PeriodStartDate = totalsByPeriod.PeriodDateStart,
                    PeriodEndDate = totalsByPeriod.PeriodDateEnd
                };
                return Ok(summaryModel);
            }
            return Ok(new SummaryModel());
        }
    }
}
