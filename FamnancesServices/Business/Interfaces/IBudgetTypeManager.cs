using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IBudgetTypeManager
    {
        IEnumerable<ExpensesBudgetType> GetAll();
        ExpensesBudgetType GetByCode(string code);
    }
}
