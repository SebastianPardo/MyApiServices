using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using FamnancesServices.Business.Interfaces;
using FamnancesServices.Models;
using Microsoft.EntityFrameworkCore;

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
                    && (e.ExpensesBudget.UserId == userId || e.ExpensesBudget.ShareOnHousehold) && e.ExpensesBudget.BudgetType.Code == "PER")
                .Select(e => new SummaryBudgetModel
                {
                    BudgetBalanceId = e.Id,
                    Id = e.ExpensesBudgetId,
                    Name = e.ExpensesBudget.Name,
                    Budget = e.Budget,
                    Spent = e.Expense,
                    UserId = e.ExpensesBudget.UserId,
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
                    var prevBudget = _context.ExpenseBudgetByPeriod.Single(e => e.Id == movement.BudgetBalanceId);

                    movement.MoveToId = movement.MoveToId == Guid.Empty ? movement.BudgetId : movement.MoveToId;
                    var budget = newBudgets.First(e => e.ExpensesBudgetId == movement.MoveToId);
                    budget.Budget = budget.Budget + (prevBudget.Budget - prevBudget.Expense);

                    _context.ExpenseBudgetByPeriod.Update(budget);

                    prevBudget.Budget = prevBudget.Expense;
                    _context.ExpenseBudgetByPeriod.Update(prevBudget);

                    _context.SaveChanges();
                }
            }
            return newBudgets;
        }

        public ExpenseBudgetByPeriod GetByIdDate(Guid userId, Guid id, DateTime date)
        {
            return _context.ExpenseBudgetByPeriod.Include(e => e.ExpensesBudget)
                .Single(e =>
                date >= e.TotalsByPeriod.PeriodDateStart
                && date <= e.TotalsByPeriod.PeriodDateEnd
                && e.ExpensesBudget.UserId == userId
                && e.ExpensesBudgetId == id);
        }
    }
}
