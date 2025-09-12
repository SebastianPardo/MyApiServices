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


        public AccountingController(
            ITotalsByPeriodManager totalsByPeriodManager,
            IUserManager userManager,
            IUtilitiesManager utilitiesManager,
            IOutflowManager outflowManager,
            IInflowManager inflowManager,
            ISavingRecordManager savingRecordManager,
            ISavingsPocketManager savingPocketManager,
            IExpensesBudgetManager expensesBudgetManager,
            IHomeManager homeManager
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
        }

        [HttpGet("CalculatePeriod")]
        public async Task<ActionResult<TotalsByPeriod>> CalculatePeriod()
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);

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
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            User user = _userManager.GetById(userId);

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
                    HomeSavings = 0,
                    Chequing = totalsByPeriod.User.TotalBudget,
                    Savings = totalsByPeriod.User.TotalSavings,
                    PeriodSavingsExpeses = totalsByPeriod.TotalSavingsExpenses,
                    SummaryBudgets = _expensesBudgetManager.GetAllByUserId(userId).Select(e => new SummaryBudgetModel
                    {
                        Name = e.Name,
                        Value = e.Value,
                        Spent = e.Outflow.Where(e => e.DateTimeStamp >= totalsByPeriod.PeriodDateStart || e.DateTimeStamp <= totalsByPeriod.PeriodDateEnd).Sum(e => e.Value)
                    }).ToList(),
                    SummaryPockets = _savingPocketManager.GetAllByUserId(userId).Select(e => new SummaryPocketModel
                    {
                        Name = e.Name,
                        Value = e.Total,
                        Spent = e.SavingsRecords.Where(e => e.IsExpense = true && e.TimeStamp >= totalsByPeriod.PeriodDateStart || e.TimeStamp <= totalsByPeriod.PeriodDateEnd).Sum(e => e.Value)
                    }).ToList()
                };

                if (user.HomeId != null)
                {
                    Home home = _homeManager.GetById(user.HomeId.Value);
                    summaryModel.HomeSavings = _savingRecordManager.GetHomeSavings(home.Id);
                    summaryModel.SummaryBudgets = _expensesBudgetManager.GetAllByHomeId(home.Id).Select(e => new SummaryBudgetModel
                    {
                        Name = e.Name,
                        Value = e.Value,
                        Spent = e.Outflow.Where(e => e.DateTimeStamp >= totalsByPeriod.PeriodDateStart || e.DateTimeStamp <= totalsByPeriod.PeriodDateEnd).Sum(e => e.Value)
                    }).ToList();
                    summaryModel.SummaryPockets = _savingPocketManager.GetAllByUserId(home.Id).Select(e => new SummaryPocketModel
                    {
                        Name = e.Name,
                        Value = e.Total,
                        Spent = e.SavingsRecords.Where(e => e.IsExpense = true && e.TimeStamp >= totalsByPeriod.PeriodDateStart || e.TimeStamp <= totalsByPeriod.PeriodDateEnd).Sum(e => e.Value)
                    }).ToList();
                }
                return Ok(summaryModel);
            }
            return Ok(new SummaryModel());
        }

    }
}
