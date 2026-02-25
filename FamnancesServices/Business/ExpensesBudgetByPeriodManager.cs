using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class ExpensesBudgetByPeriodManager : IExpensesBudgetByPeriodManager
    {
        DatabaseContext _context;
        public ExpensesBudgetByPeriodManager(DatabaseContext context)
        {
            _context = context;
        }

        public List<SummaryBudgetModel> ExpensesBudgetsSummary(Guid userId, DateTime dateTime)
        {
            return _context.ExpenseBudgetByPeriod
                .Where(e => dateTime >= e.TotalsByPeriod.PeriodDateStart && dateTime <= e.TotalsByPeriod.PeriodDateEnd
                    && (e.ExpensesBudget.UserId == userId || e.ExpensesBudget.ShareOnHousehold))
                .Select(e => new SummaryBudgetModel
                {
                    Id = e.ExpensesBudgetId,
                    Name = e.ExpensesBudget.Name,
                    Budget = e.Budget,
                    Spent = e.Expense,
                    UserId  = e.ExpensesBudget.UserId,
                    UserName = e.ExpensesBudget.User.FirstName
                }).ToList();
        }

        public List<ExpenseBudgetByPeriod> VeryFirst(Guid userId, Guid TotalByPeriodId)
        {
            List<ExpenseBudgetByPeriod> newBudgets = new List<ExpenseBudgetByPeriod>();
            var budgets = _context.ExpensesBudget.Where(e => e.UserId == userId);
            foreach (var budget in budgets)
            {
                ExpenseBudgetByPeriod expenseBudgetByPeriod = new ExpenseBudgetByPeriod
                {
                    ExpensesBudgetId = budget.Id,
                    TotalsByPeriodId = TotalByPeriodId,
                    Budget = budget.Value,
                    Id = Guid.NewGuid()
                };
                newBudgets.Add(expenseBudgetByPeriod);
            }
            _context.ExpenseBudgetByPeriod.AddRange(newBudgets);
            _context.SaveChanges();
            return newBudgets;
        }

        public List<ExpenseBudgetByPeriod> CalculateNew(Guid userId, Guid totalByPeriodId, List<RemainderBalance> remaindersBalance)
        {
            var newBudgets = VeryFirst(userId, totalByPeriodId);
            foreach (var movement in remaindersBalance)
            {
                if (!movement.IsSavingPocket)
                {
                    var prevBudget = _context.ExpenseBudgetByPeriod.First(e => e.Id == movement.BudgesTotalstId);
                    var budget = newBudgets.First(e => e.ExpensesBudgetId == movement.MoveToId);
                    budget.Budget = budget.Budget + (prevBudget.Budget - prevBudget.Expense);
                    _context.ExpenseBudgetByPeriod.Update(budget);
                }
            }
            return newBudgets;
        }
    }
}
