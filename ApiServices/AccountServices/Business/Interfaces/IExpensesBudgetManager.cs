﻿using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business.Interfaces
{
    public interface IExpensesBudgetManager
    {
        IEnumerable<ExpensesBudget> GetAllByUserId(Guid userId);
        ExpensesBudget GetById(Guid id);
        ExpensesBudget Add(ExpensesBudget expensesBudget);
        bool Update(ExpensesBudget expensesBudget);
        bool Delete(ExpensesBudget expensesBudget);
    }
}
