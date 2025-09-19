using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IFixedExpenseManager
    {
        IEnumerable<FixedExpense> GetAllByUserId(Guid userId);
        FixedExpense GetById(Guid id);
        FixedExpense Add(FixedExpense fixedExpense);
        bool Update(FixedExpense fixedExpense);
        bool Delete(FixedExpense fixedExpense);
        IEnumerable<FixedExpense> GetAllByHome(Guid id);
    }
}
