using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace FamnancesServices.Business
{
    public class ExpensesBudgetManager : IExpensesBudgetManager
    {
        DatabaseContext context;
        public ExpensesBudgetManager(DatabaseContext context)
        {
            this.context = context;
        }

        public ExpensesBudget Add(ExpensesBudget expensesBudget)
        {
            expensesBudget = context.ExpensesBudget.Add(expensesBudget).Entity;
            context.SaveChanges();
            return expensesBudget;
        }

        public bool Delete(ExpensesBudget expensesBudget)
        {
            context.ExpensesBudget.Remove(expensesBudget);
            return context.SaveChanges() > 0;
        }

        public List<ExpensesBudget> GetAllByHomeId(Guid homeId)
        {
            return context.ExpensesBudget
                .Include(e=>e.BudgetType)
                .Include(e => e.Outflow)
                .Where(e => e.User.HomeId == homeId && e.BudgetType.Code == "PER").ToList();
        }

        public List<ExpensesBudget> GetAllByUserId(Guid userId)
        {
            return context.ExpensesBudget
                .Include(e => e.BudgetType)
                .Include(e => e.Outflow)
                .Where(e => e.UserId == userId && e.BudgetType.Code == "PER").ToList();
        }

        public List<ExpensesBudget> GetByType(string typeCode, Guid userId)
        {
            return context.ExpensesBudget.Where(e => e.BudgetType.Code == typeCode && e.UserId == userId).ToList();
        }

        public ExpensesBudget GetById(Guid id)
        {
            return context.ExpensesBudget.FirstOrDefault(x => x.Id == id);
        }

        public bool Update(ExpensesBudget expensesBudget)
        {
            context.ExpensesBudget.Update(expensesBudget);
            return context.SaveChanges() > 0;
        }
    }
}
