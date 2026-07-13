using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IFixedExpensePaymentRecordManager
    {
        FixedExpensePaymentRecord GetById(Guid id);
        IEnumerable<FixedExpensePaymentRecord>? GetByFixedExpenseId(Guid fixedExpenseId);
        FixedExpensePaymentRecord Add(FixedExpensePaymentRecord fixedExpense);
        bool Delete(FixedExpensePaymentRecord fixedExpense);        
        bool Update(FixedExpensePaymentRecord fixedExpense);
    }
}