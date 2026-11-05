using Famnances.Core.Security;
using Famnances.Core.Security.Authorization;
using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using FamnancesServices.Business.Interfaces;
using FamnancesServices.Models;
using Microsoft.AspNetCore.Mvc;

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
        IFixedExpenseManager _fixedExpenseManager;
        IExpensesBudgetByPeriodManager _expensesBudgetByPeriodManager;
        IHomeManager _homeManager;
        IFixedSavingManager _fixedSavingsManager;

        IFinancialAccountManager _financialAccountManager;


        public AccountingController(
            ITotalsByPeriodManager totalsByPeriodManager,
            IUserManager userManager,
            IUtilitiesManager utilitiesManager,
            IOutflowManager outflowManager,
            IInflowManager inflowManager,
            ISavingRecordManager savingRecordManager,
            ISavingsPocketManager savingPocketManager,
            IFixedExpenseManager fixedExpenseManager,
            IExpensesBudgetByPeriodManager expensesBudgetByPeriodManager,
            IHomeManager homeManager,
            IFixedSavingManager fixedSavingsManager,
            IFinancialAccountManager financialAccountManager
            )
        {
            _totalsByPeriodManager = totalsByPeriodManager;
            _userManager = userManager;
            _utilitiesManager = utilitiesManager;
            _outflowManager = outflowManager;
            _inflowManager = inflowManager;
            _savingRecordManager = savingRecordManager;
            _savingPocketManager = savingPocketManager;
            _fixedExpenseManager = fixedExpenseManager;
            _expensesBudgetByPeriodManager = expensesBudgetByPeriodManager;
            _homeManager = homeManager;
            _fixedSavingsManager = fixedSavingsManager;
            _financialAccountManager = financialAccountManager;
        }
        
        [HttpGet("PeriodSummary/{date}")]
        public async Task<ActionResult<SummaryModel>> PeriodSummary(DateTime? date)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            User user = _userManager.GetById(userId);
            user.FinancialAccounts = _financialAccountManager.GetByUserId(userId).ToList();

            PeriodDates? periodDates = date == null? 
                _totalsByPeriodManager.GetByCurrentDay(userId) : _totalsByPeriodManager.GetByDate(userId, date.Value);

            if (periodDates != null)
            {
                decimal balance = user.BudgetByPeriod - periodDates.FinancialAccountsByPeriodDates.Where(e=>e.FinancialAccount.FinancialAccountType.Code == "CA").Sum(e => e.TotalWithdrawals);

                var budgetsByUser = _expensesBudgetByPeriodManager.ExpensesBudgetsSummary(userId, user.HomeId, date?? DateTimeEast.Now).ToLookup(b => b.UserId);
                var pocketsByUser = _savingPocketManager.Summary(userId, user.HomeId, date ?? DateTimeEast.Now).ToLookup(p => p.UserId);
                var fixedsByUser = _fixedExpenseManager.Summary(userId, user.HomeId, date ?? DateTimeEast.Now).ToLookup(f => f.UserId);
                var fixedSavingsByUser = _fixedSavingsManager.Summary(userId, user.HomeId, date ?? DateTimeEast.Now).ToLookup(f => f.UserId);

                var homeMembers = user.HomeId != null ? 
                    _homeManager.GetById(user.HomeId.Value).Users.ToList().ToLookup(e => e.Id)
                    : new List<User> { user }.ToLookup(e => e.Id);

                var roommates = homeMembers
                    .Select(g => new RoommateModel
                    {
                        Name = g.First().FirstName,
                        IsCurrentUser = g.Key == userId,
                        SummaryBudgets = budgetsByUser[g.Key].ToList(),
                        SummaryPockets = pocketsByUser[g.Key].ToList(),
                        SummaryFixedExpenses = fixedsByUser[g.Key].ToList(),
                        SummaryFixedSavings = fixedSavingsByUser[g.Key].ToList()
                    }).OrderByDescending(r => r.IsCurrentUser)
                    .ToList();
                                
                SummaryModel summaryModel = new SummaryModel
                {
                    PeriodBudget = periodDates.User.BudgetByPeriod,
                    PeriodSpent = periodDates.FinancialAccountsByPeriodDates.Where(e => e.FinancialAccount.FinancialAccountType.Code == "CA").Sum(e => e.TotalWithdrawals),
                    HomeSavings = user.HomeId == null ? 0 : _savingRecordManager.GetHomeSavings(user.HomeId.Value),
                    Chequing = periodDates.User.FinancialAccounts.Where(e => e.FinancialAccountType.Code == "CA").Sum(e => e.Amount),
                    Savings = periodDates.User.FinancialAccounts.Where(e => e.FinancialAccountType.Code == "HISA").Sum(e => e.Amount),
                    PeriodSavingsSpent = periodDates.FinancialAccountsByPeriodDates.Where(e => e.FinancialAccount.FinancialAccountType.Code == "HISA").Sum(e => e.TotalWithdrawals),
                    Roommates = roommates,
                };               
                return Ok(summaryModel);
            }
            return Ok(new SummaryModel());
        }

        [HttpGet("PeriodMiniSummary/{date}")]
        public async Task<ActionResult<MiniSummaryModel>> PeriodMiniSummary(DateTime? date)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            User user = _userManager.GetById(userId);
            PeriodDates? PeriodDates = date == null ?
                _totalsByPeriodManager.GetByCurrentDay(userId) : _totalsByPeriodManager.GetByDate(userId, date.Value);

            if (PeriodDates != null) {
                MiniSummaryModel summaryModel = new MiniSummaryModel
                {
                    ToBeClosed = !_totalsByPeriodManager.Exist(userId, PeriodDates.PeriodDateEnd.AddDays(1))
                                    && (DateTimeEast.Now >= PeriodDates.PeriodDateEnd.AddDays(-3) && DateTimeEast.Now <= PeriodDates.PeriodDateEnd),
                    PeriodFrom = PeriodDates.PeriodDateStart,
                    PeriodTo = PeriodDates.PeriodDateEnd,
                    Chequing = _financialAccountManager.GetByUserId(userId).Where(e => e.FinancialAccountType.Code == "CA").Sum(e => e.Amount),
                    Savings = _financialAccountManager.GetByUserId(userId).Where(e => e.FinancialAccountType.Code == "HISA").Sum(e => e.Amount),
                };
                return Ok(summaryModel);
            }

            return Ok(null);
        }

        [HttpPost("ClosePeriod")]
        public async Task<ActionResult<Guid>> ClosePeriod(List<RemainderBalance> remainderBalance)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            User user = _userManager.GetById(Guid.Parse(accountId.ToString()));

            PeriodDates nextPeriod = _totalsByPeriodManager.GetByDate(user.Id, DateTimeEast.Now.AddDays(3));
            PeriodDates PeriodDates = _totalsByPeriodManager.GetByCurrentDay(user.Id);
            if (nextPeriod == null )
            {
                PeriodDates? prevTotalsByPeriod = _totalsByPeriodManager.GetMostRecent(user.Id);
                PeriodDates = GetNewPeriod(user);
                if (prevTotalsByPeriod == null || prevTotalsByPeriod.Id == PeriodDates.Id)
                {
                    _expensesBudgetByPeriodManager.VeryFirst(user.Id, PeriodDates.Id);
                }
                else
                {
                    _expensesBudgetByPeriodManager.CalculateNew(user.Id, PeriodDates.Id, remainderBalance.Where(e => e.IsSavingPocket == false).ToList());
                    _savingRecordManager.ClosePeriod(remainderBalance);
                }
            }
            return PeriodDates.Id;
        }

        private PeriodDates GetNewPeriod(User user)
        {
            var periodDates = _utilitiesManager.GetPeriodDates(user.PeriodId, user.PeriodStartsMonthsDay, DateTimeEast.Now.AddDays(3));
            PeriodDates PeriodDates = new PeriodDates
            {
                Id = Guid.NewGuid(),
                PeriodDateStart = periodDates.Item1,
                PeriodDateEnd = periodDates.Item2,
                //PeriodActive = true,
                //TotalExpenses = _outflowManager.GetByPeriod(periodDates.Item1, periodDates.Item2, user.Id),
                //TotalIncomes = _inflowManager.GetTotalByPeriod(periodDates.Item1, periodDates.Item2, user.Id),
                //TotalSavings = _savingRecordManager.GetSavingsIncomeByPeriod(periodDates.Item1, periodDates.Item2, user.Id),
                //TotalSavingsExpenses = _savingRecordManager.GetSavingsExpensesByPeriod(periodDates.Item1, periodDates.Item2, user.Id),
                UserId = user.Id
            };
            return _totalsByPeriodManager.Save(PeriodDates);
        }

    }
}
