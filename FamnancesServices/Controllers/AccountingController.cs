using Famnances.AuthMiddleware;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using FamnancesServices.Business;
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
        IUserManager _userManager;
        IUtilitiesManager _utilitiesManager;
        IOutflowManager _outflowManager;
        IInflowManager _inflowManager;
        ISavingRecordManager _savingRecordManager;


        public AccountingController(
            ITotalsByPeriodManager totalsByPeriodManager,
            IUserManager userManager,
            IUtilitiesManager utilitiesManager,
            IOutflowManager outflowManager,
            IInflowManager inflowManager,
            ISavingRecordManager savingRecordManager
            )
        {
            _totalsByPeriodManager = totalsByPeriodManager;
            _userManager = userManager;
            _utilitiesManager = utilitiesManager;
            _outflowManager = outflowManager;
            _inflowManager = inflowManager;
            _savingRecordManager = savingRecordManager;
        }

        [HttpGet("CalculatePeriod")]
        public IActionResult CalculatePeriod()
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);

            User user = _userManager.GetById(Guid.Parse(accountId.ToString()));
            TotalsByPeriod totalsByPeriod = _totalsByPeriodManager.GetByCurrentDay(user.Id);

            if(totalsByPeriod == null)
            {
                var periodDates = _utilitiesManager.GetPeriodDates(user.PeriodId, user.PeriodStartsMonthsDay);
                totalsByPeriod = new TotalsByPeriod
                {
                    Id = Guid.NewGuid(),
                    PeriodDateStart = periodDates.Item1,
                    PeriodDateEnd = periodDates.Item2,
                    PeriodActive = true,
                    TotalExpenses = _outflowManager.GetByPeriod(periodDates.Item1, periodDates.Item2),
                    TotalIncomes = _inflowManager.GetTotalByPeriod(periodDates.Item1, periodDates.Item2, user.Id),
                    TotalSavings = _savingRecordManager.GetSavingsIncomeByPeriod(periodDates.Item1, periodDates.Item2),
                    TotalSavingsExpenses = _savingRecordManager.GetSavingsExpensesByPeriod(periodDates.Item1, periodDates.Item2),
                    UserId = user.Id
                };
                _totalsByPeriodManager.Save(totalsByPeriod);
            }
            return Ok(totalsByPeriod);
        }

        [HttpGet("CurentTotals")]
        public async Task<IActionResult> CurentTotals()
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            //var accountId = account.GetType().GetProperty("Id").GetValue(account);

            TotalsByPeriod? totalsByPeriod = _totalsByPeriodManager.GetByCurrentPeriod(Guid.Parse(accountId.ToString()));
            if (totalsByPeriod != null)
            {
                decimal balance = totalsByPeriod.User.BudgetByPeriod - totalsByPeriod.TotalExpenses;
                SummaryModel summaryModel = new SummaryModel
                {
                    PeriodStartDate = totalsByPeriod.PeriodDateStart,
                    PeriodEndDate = totalsByPeriod.PeriodDateEnd,
                    PeriodBudget = totalsByPeriod.User.BudgetByPeriod,
                    PeriodBalance = balance,
                    PeriodExpenses = totalsByPeriod.TotalExpenses,
                    HomeSavings = 0,
                    Chequing = totalsByPeriod.User.TotalBudget,
                    Savings = totalsByPeriod.User.TotalSavings,
                    PeriodSavingsExpeses = totalsByPeriod.TotalSavingsExpenses
                };
                return Ok(summaryModel);
            }
            return Ok(new SummaryModel());
        }

    }
}
