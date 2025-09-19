using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IBudgetTypeManager
    {
        ExpensesBudgetType GetByCode(string code);
    }
}
