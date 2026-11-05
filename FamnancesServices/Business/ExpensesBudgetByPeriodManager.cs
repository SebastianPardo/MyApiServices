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

        public List<SummaryBudgetModel> ExpensesBudgetsSummary(Guid userId, Guid? householdId, DateTime dateTime)
        {
            return _context.ExpenseBudgetByPeriodDates
                .Where(e => dateTime >= e.PeriodsDates.PeriodDateStart && dateTime <= e.PeriodsDates.PeriodDateEnd
                    && (e.ExpensesBudget.FinancialAccount.UserId == userId || (e.ExpensesBudget.ShareOnHousehold && e.ExpensesBudget.FinancialAccount.User.HomeId == householdId )) 
                    && e.ExpensesBudget.BudgetType.Code == "PER" && e.ExpensesBudget.Active == true)
                .Select(e => new SummaryBudgetModel
                {
                    BudgetPeriodBalanceId = e.Id,
                    Id = e.ExpensesBudgetId,
                    Name = e.ExpensesBudget.Name,
                    Budget = e.TotalDeposits,
                    Spent = e.TotalWithdrawals,
                    UserId = e.ExpensesBudget.FinancialAccount.UserId,
                    UserName = e.ExpensesBudget.FinancialAccount.User.FirstName
                }).ToList();
        }

        public List<ExpenseBudgetByPeriodDates> VeryFirst(Guid userId, Guid TotalByPeriodId)
        {
            List<ExpenseBudgetByPeriodDates> newBudgets = new List<ExpenseBudgetByPeriodDates>();
            var budgets = _context.ExpensesBudget.Where(e => e.FinancialAccount.UserId == userId);
            foreach (var budget in budgets)
            {
                ExpenseBudgetByPeriodDates ExpenseBudgetByPeriodDates = new ExpenseBudgetByPeriodDates
                {
                    ExpensesBudgetId = budget.Id,
                    PeriodDatesId = TotalByPeriodId,
                    TotalDeposits = budget.Value,
                    Id = Guid.NewGuid()
                };
                newBudgets.Add(ExpenseBudgetByPeriodDates);
            }
            _context.ExpenseBudgetByPeriodDates.AddRange(newBudgets);
            _context.SaveChanges();
            return newBudgets;
        }

        public List<ExpenseBudgetByPeriodDates> CalculateNew(Guid userId, Guid totalByPeriodId, List<RemainderBalance> remaindersBalance)
        {
            var newBudgets = VeryFirst(userId, totalByPeriodId);
            foreach (var movement in remaindersBalance)
            {
                if (!movement.IsSavingPocket)
                {
                    var prevBudget = _context.ExpenseBudgetByPeriodDates.Single(e => e.Id == movement.BudgetBalanceId);

                    movement.MoveToId = movement.MoveToId == Guid.Empty ? movement.BudgetId : movement.MoveToId;
                    var budget = newBudgets.First(e => e.ExpensesBudgetId == movement.MoveToId);
                    budget.TotalDeposits = budget.TotalDeposits + (prevBudget.TotalDeposits - prevBudget.TotalWithdrawals);

                    _context.ExpenseBudgetByPeriodDates.Update(budget);

                    prevBudget.TotalDeposits = prevBudget.TotalWithdrawals;
                    _context.ExpenseBudgetByPeriodDates.Update(prevBudget);

                    _context.SaveChanges();
                }
            }
            return newBudgets;
        }

        public ExpenseBudgetByPeriodDates GetByIdDate(Guid userId, Guid id, DateTime date)
        {
            return _context.ExpenseBudgetByPeriodDates.Include(e => e.ExpensesBudget)
                .Single(e =>
                date >= e.PeriodsDates.PeriodDateStart
                && date <= e.PeriodsDates.PeriodDateEnd
                && e.ExpensesBudget.FinancialAccount.UserId == userId
                && e.ExpensesBudgetId == id);
        }
    }
}
