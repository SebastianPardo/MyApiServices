using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using FamnancesServices.Models;

namespace FamnancesServices.Business.Interfaces
{
    public interface IExpensesBudgetByPeriodManager
    {
        List<SummaryBudgetModel> ExpensesBudgetsSummary(Guid userId, Guid? householdId, DateTime date);
        List<ExpenseBudgetByPeriodDates> VeryFirst(Guid userId, Guid totalByPeriodId);
        List<ExpenseBudgetByPeriodDates> CalculateNew(Guid userId, Guid totalByPeriodId, List<RemainderBalance> remaindersBalance);
        ExpenseBudgetByPeriodDates GetByIdDate(Guid userId, Guid id, DateTime date);
    }
}
