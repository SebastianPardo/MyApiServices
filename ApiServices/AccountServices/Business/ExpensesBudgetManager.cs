﻿using AccountServices.Business.Interfaces;
using OhMyMoney.DataCore.Data;
using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business
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


        public bool Update(ExpensesBudget expensesBudget)
        {
            context.ExpensesBudget.Update(expensesBudget);
            context.SaveChanges();
            return context.SaveChanges() > 0;
        }

        IEnumerable<ExpensesBudget> IExpensesBudgetManager.GetAllByUserId(Guid userId)
        {
            return context.ExpensesBudget.Where(fe => fe.UserId == userId);
        }

        ExpensesBudget IExpensesBudgetManager.GetById(Guid id)
        {
            return context.ExpensesBudget.FirstOrDefault(x => x.Id == id);
        }
    }
}
