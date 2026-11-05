using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IFinancialAccountByPeriodDatesManager
    {
        IEnumerable<FinancialAccountByPeriodDates> GetByUserId(Guid userId);
    }
}