using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            return context.ExpensesBudget.Include(e => e.Outflow).Where(fe => fe.User.HomeId == homeId).ToList();
        }

        public List<ExpensesBudget> GetAllByUserId(Guid userId)
        {
            return context.ExpensesBudget.Include(e => e.Outflow).Where(fe => fe.UserId == userId).ToList();
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
