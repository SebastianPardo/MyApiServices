using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business.Interfaces
{
    public interface IFixedExpenseManager
    {
        IEnumerable<FixedExpense> GetAllByUserId(Guid userId);
        FixedExpense GetById(Guid id);
        FixedExpense Add(FixedExpense fixedExpense);
        bool Update(FixedExpense fixedExpense);
        bool Delete(FixedExpense fixedExpense);
    }
}
