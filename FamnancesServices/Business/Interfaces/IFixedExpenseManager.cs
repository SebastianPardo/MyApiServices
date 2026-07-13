using Famnances.DataCore.Entities;
using FamnancesServices.Models;

namespace FamnancesServices.Business.Interfaces
{
    public interface IFixedExpenseManager
    {
        IEnumerable<FixedExpense> GetAllByUserId(Guid userId);
        FixedExpense? GetById(Guid userId, Guid id);
        FixedExpense GetCompleteByIdDates(Guid userId, Guid id, DateTime from, DateTime to);
        IEnumerable<FixedExpense> GetAllByHome(Guid id);
        List<SummaryFixedExpensesModel> Summary(Guid userId, Guid? householdId, DateTime dateTime);
        FixedExpense Add(FixedExpense fixedExpense);
        bool Update(FixedExpense fixedExpense);
        bool Delete(FixedExpense fixedExpense);
    }
}
