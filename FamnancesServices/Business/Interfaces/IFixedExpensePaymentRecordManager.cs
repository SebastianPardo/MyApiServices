using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IFixedExpensePaymentRecordManager
    {
        FixedExpensePaymentRecord Add(FixedExpensePaymentRecord fixedExpense);
        bool Delete(FixedExpensePaymentRecord fixedExpense);
        FixedExpensePaymentRecord GetById(Guid id);
        bool Update(FixedExpensePaymentRecord fixedExpense);
    }
}