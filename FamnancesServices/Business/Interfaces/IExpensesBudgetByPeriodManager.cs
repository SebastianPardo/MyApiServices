using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;

namespace FamnancesServices.Business.Interfaces
{
    public interface IExpensesBudgetByPeriodManager
    {
        List<ExpenseBudgetByPeriod> VeryFirst(Guid userId, Guid totalByPeriodId);
        List<ExpenseBudgetByPeriod> CalculateNew(Guid userId, Guid totalByPeriodId, List<RemainderBalance> remaindersBalance);
    }
}
