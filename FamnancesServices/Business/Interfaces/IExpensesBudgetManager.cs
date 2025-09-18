using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IExpensesBudgetManager
    {
        List<ExpensesBudget> GetAllByUserId(Guid userId);
        List<ExpensesBudget> GetAllByHomeId(Guid homeId);
        ExpensesBudget GetById(Guid id);
        List<ExpensesBudget> GetByType(string typeCode, Guid userId);
        ExpensesBudget Add(ExpensesBudget expensesBudget);
        bool Update(ExpensesBudget expensesBudget);
        bool Delete(ExpensesBudget expensesBudget);
    }
}
