using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IFinancialAccountManager
    {
        IEnumerable<FinancialAccount> GetByUserId(Guid userId);
    }
}