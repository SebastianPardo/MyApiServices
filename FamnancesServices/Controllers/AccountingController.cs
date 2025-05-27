using Famnances.AuthMiddleware;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class AccountingController : ControllerBase
    {
        ITotalsByPeriodManager _totalsByPeriodManager;
        IFixedIncomeManager _fixedIncomeManager;
        IIncomeDiscountManager _incomeDiscountManager;

        public AccountingController(
            ITotalsByPeriodManager totalsByPeriodManager,
            IFixedIncomeManager fixedIncomeManager,
            IIncomeDiscountManager incomeDiscountManager
            )
        {
            _totalsByPeriodManager = totalsByPeriodManager;
            _fixedIncomeManager = fixedIncomeManager;
            _incomeDiscountManager = incomeDiscountManager;
        }

        [HttpGet("CurentTotals")]
        public async Task<IActionResult> CurentTotals()
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            //var accountId = account.GetType().GetProperty("Id").GetValue(account);
            
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

        [HttpGet("GetFixedIncomes")]
        public async Task<ActionResult> GetFixedIncomes()
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var fixedIncomes = _fixedIncomeManager.GetAllByUserId(userId);
            return Ok(fixedIncomes);
        }

        [HttpPost("AddFixedIncome")]
        public async Task<IActionResult> AddFixedIncome(FixedIncome entity)
        {
            HttpContext.Items.TryGetValue("AccountId", out var accountId);
            entity.UserId = Guid.Parse(accountId.ToString());
            entity = _fixedIncomeManager.Add(entity);
            return Ok(entity);
        }

        [HttpGet("GetIncomeDiscounts")]
        public async Task<IActionResult> GetIncomeDiscounts()
        {
            HttpContext.Items.TryGetValue("AccountId", out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            IEnumerable<IncomeDiscount> entity = _incomeDiscountManager.GetAllByUser(userId);
            return Ok(entity);
        }

        [HttpPost("AddIncomeDiscount")]
        public async Task<IActionResult> AddIncomeDiscount(IncomeDiscount entity)
        {
            HttpContext.Items.TryGetValue("AccountId", out var accountId);
            entity.UserId = Guid.Parse(accountId.ToString());
            entity = _incomeDiscountManager.Add(entity);
            return Ok(entity);
        }

        [HttpPost("UpdateIncomeDiscount")]
        public async Task<IActionResult> UpdateIncomeDiscount(IncomeDiscount entity)
        {
            HttpContext.Items.TryGetValue("AccountId", out var accountId);
            entity.UserId = Guid.Parse(accountId.ToString());
            _incomeDiscountManager.Update(entity);
            return Ok(entity);
        }
    }
}
