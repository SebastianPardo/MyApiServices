using Famnances.Core.Security;
using Famnances.Core.Security.Authorization;
using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Net.Sockets;

namespace FamnancesServices.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
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
        IExpensesBudgetByPeriodManager _expensesBudgetByPeriodManager;


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
            IFixedExpenseManager fixedExpenseManager,
            IExpensesBudgetByPeriodManager expensesBudgetByPeriodManager
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
            _expensesBudgetByPeriodManager = expensesBudgetByPeriodManager;
        }

        private TotalsByPeriod GetNew(User user)
        {
            var periodDates = _utilitiesManager.GetPeriodDates(user.PeriodId, user.PeriodStartsMonthsDay);
            TotalsByPeriod totalsByPeriod = new TotalsByPeriod
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
            return _totalsByPeriodManager.Save(totalsByPeriod);
        }

        [HttpPost("ClosePeriod")]
        public async Task<ActionResult<Guid>> ClosePeriod(List<RemainderBalance> remainderBalance)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            User user = _userManager.GetById(Guid.Parse(accountId.ToString()));

            TotalsByPeriod totalsByPeriod = _totalsByPeriodManager.GetByCurrentDay(user.Id);
            if (totalsByPeriod == null)
            {
                totalsByPeriod = GetNew(user);
                TotalsByPeriod prevTotalsByPeriod = _totalsByPeriodManager.GetMostRecent(user.Id);
                if (prevTotalsByPeriod == null) 
                {
                    _expensesBudgetByPeriodManager.VeryFirst(user.Id, totalsByPeriod.Id);
                }
                else
                {
                    _expensesBudgetByPeriodManager.CalculateNew(user.Id, totalsByPeriod.Id, remainderBalance.Where(e => e.IsSavingPocket == false).ToList());
                    _savingRecordManager.ClosePeriod(remainderBalance);
                }
            }
            return totalsByPeriod.Id;
        }

        [HttpGet("CurentTotals/{date}")]
        public async Task<ActionResult<SummaryModel>> CurentTotals(DateTime? date)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            User user = _userManager.GetById(userId);

            TotalsByPeriod? totalsByPeriod = date == null? 
                _totalsByPeriodManager.GetByCurrentDay(userId) : _totalsByPeriodManager.GetByDate(userId, date.Value);
            
            if (totalsByPeriod != null)
            {
                decimal balance = user.BudgetByPeriod - totalsByPeriod.TotalExpenses;

                var budgetsByUser = _expensesBudgetByPeriodManager.ExpensesBudgetsSummary(userId, date?? DateTimeEast.Now).ToLookup(b => b.UserId);
                var pocketsByUser = _savingPocketManager.Summary(userId, date ?? DateTimeEast.Now).ToLookup(p => p.UserId);
                var fixedsByUser = _fixedExpenseManager.Summary(userId, date ?? DateTimeEast.Now).ToLookup(f => f.UserId);

                var roommates = budgetsByUser
                    .Select(g => new RoommateModel
                    {
                        Name = g.First().UserName,
                        IsCurrentUser = g.Key == userId,
                        SummaryBudgets = g.ToList(),
                        SummaryPockets = pocketsByUser[g.Key].ToList(),
                        SummaryFixedExpenses = fixedsByUser[g.Key].ToList()
                    }).OrderByDescending(r => r.IsCurrentUser)
                    .ToList();

                SummaryModel summaryModel = new SummaryModel
                {
                    PeriodStartDate = totalsByPeriod.PeriodDateStart,
                    PeriodEndDate = totalsByPeriod.PeriodDateEnd,
                    PeriodBudget = totalsByPeriod.User.BudgetByPeriod,
                    PeriodBalance = balance,
                    PeriodExpenses = totalsByPeriod.TotalExpenses,
                    HomeSavings = user.HomeId == null ? 0 : _savingRecordManager.GetHomeSavings(user.HomeId.Value),
                    Chequing = totalsByPeriod.User.TotalBudget,
                    Savings = totalsByPeriod.User.TotalSavings,
                    PeriodSavingsExpeses = totalsByPeriod.TotalSavingsExpenses,
                    Roommates = roommates,
                };               
                return Ok(summaryModel);
            }
            return Ok(new SummaryModel());
        }


        [HttpGet("GetHeaderSummary/{date}")]
        public async Task<ActionResult<MiniSummaryModel>> GetHeaderSummary(DateTime? date)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            User user = _userManager.GetById(userId);
            TotalsByPeriod? totalsByPeriod = date == null ?
                _totalsByPeriodManager.GetByCurrentDay(userId) : _totalsByPeriodManager.GetByDate(userId, date.Value);

            if (totalsByPeriod != null) {
                MiniSummaryModel summaryModel = new MiniSummaryModel
                {
                    PeriodFrom = totalsByPeriod.PeriodDateStart,
                    PeriodTo = totalsByPeriod.PeriodDateEnd,
                    Chequing = user.TotalBudget,
                    Savings = user.TotalSavings,
                };
                return Ok(summaryModel);
            }

            return Ok(null);
        }

    }
}
