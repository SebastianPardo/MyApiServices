using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IFinancialAccountManager
    {
        IEnumerable<FinancialAccount> GetByUserId(Guid userId);
        IEnumerable<FinancialAccount> GetAll(Guid userId);
        IEnumerable<FinancialAccount> GetByType(Guid userId, string code);
    }
}