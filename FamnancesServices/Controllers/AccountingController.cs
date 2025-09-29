using Famnances.Core.Security;
using Famnances.Core.Security.Authorization;
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
        IUserManager _userManager;
        IUtilitiesManager _utilitiesManager;
        IOutflowManager _outflowManager;
        IInflowManager _inflowManager;
        ISavingRecordManager _savingRecordManager;
        ISavingsPocketManager _savingPocketManager;
        IExpensesBudgetManager _expensesBudgetManager;
        IHomeManager _homeManager;
        IFixedExpenseManager _fixedExpenseManager;


        public AccountingController(
            ITotalsByPeriodManager totalsByPeriodManager,
            IUserManager userManager,
            IUtilitiesManager utilitiesManager,
            IOutflowManager outflowManager,
            IInflowManager inflowManager,
            ISavingRecordManager savingRecordManager,
            ISavingsPocketManager savingPocketManager,
            IExpensesBudgetManager expensesBudgetManager,
            IHomeManager homeManager,
            IFixedExpenseManager fixedExpenseManager
            )
        {
            _totalsByPeriodManager = totalsByPeriodManager;
            _userManager = userManager;
            _utilitiesManager = utilitiesManager;
            _outflowManager = outflowManager;
            _inflowManager = inflowManager;
            _savingRecordManager = savingRecordManager;
            _savingPocketManager = savingPocketManager;
            _expensesBudgetManager = expensesBudgetManager;
            _homeManager = homeManager;
            _fixedExpenseManager = fixedExpenseManager;
        }

        [HttpGet("CalculatePeriod")]
        public async Task<ActionResult<TotalsByPeriod>> CalculatePeriod()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);

            User user = _userManager.GetById(Guid.Parse(accountId.ToString()));
            TotalsByPeriod totalsByPeriod = _totalsByPeriodManager.GetByCurrentDay(user.Id);

            if (totalsByPeriod == null)
            {
                var periodDates = _utilitiesManager.GetPeriodDates(user.PeriodId, user.PeriodStartsMonthsDay);
                totalsByPeriod = new TotalsByPeriod
                {
                    Id = Guid.NewGuid(),
                    PeriodDateStart = periodDates.Item1,
                    PeriodDateEnd = periodDates.Item2,
                    PeriodActive = true,
                    TotalExpenses = _outflowManager.GetByPeriod(periodDates.Item1, periodDates.Item2, user.Id),
                    TotalIncomes = _inflowManager.GetTotalByPeriod(periodDates.Item1, periodDates.Item2, user.Id),
                    TotalSavings = _savingRecordManager.GetSavingsIncomeByPeriod(periodDates.Item1, periodDates.Item2, user.Id),
                    TotalSavingsExpenses = _savingRecordManager.GetSavingsExpensesByPeriod(periodDates.Item1, periodDates.Item2, user.Id),
                    UserId = user.Id
                };
                _totalsByPeriodManager.Save(totalsByPeriod);
            }
            return Ok(totalsByPeriod);
        }

        [HttpGet("CurentTotals")]
        public async Task<ActionResult<SummaryModel>> CurentTotals()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            User user = _userManager.GetById(userId);
            Home home = _homeManager.GetComplete(userId);

            TotalsByPeriod? totalsByPeriod = _totalsByPeriodManager.GetByCurrentPeriod(userId);
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
                    HomeSavings = _savingRecordManager.GetHomeSavings(home.Id),
                    Chequing = totalsByPeriod.User.TotalBudget,
                    Savings = totalsByPeriod.User.TotalSavings,
                    PeriodSavingsExpeses = totalsByPeriod.TotalSavingsExpenses,
                    Roommates = _homeManager.GetComplete(userId).Users.Select(e => new RoommateModel(e)).ToList(),
                };               
                return Ok(summaryModel);
            }
            return Ok(new SummaryModel());
        }

    }
}
