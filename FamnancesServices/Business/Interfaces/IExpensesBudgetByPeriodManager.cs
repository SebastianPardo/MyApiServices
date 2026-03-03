using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using FamnancesServices.Models;

namespace FamnancesServices.Business.Interfaces
{
    public interface IExpensesBudgetByPeriodManager
    {
        List<SummaryBudgetModel> ExpensesBudgetsSummary(Guid userId, DateTime date);
        List<ExpenseBudgetByPeriod> VeryFirst(Guid userId, Guid totalByPeriodId);
        List<ExpenseBudgetByPeriod> CalculateNew(Guid userId, Guid totalByPeriodId, List<RemainderBalance> remaindersBalance);
    }
}
