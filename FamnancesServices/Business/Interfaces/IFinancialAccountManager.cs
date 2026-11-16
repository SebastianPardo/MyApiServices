using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IFinancialAccountManager
    {
        IEnumerable<FinancialAccount> GetByUserId(Guid userId);
        IEnumerable<FinancialAccount> GetByFinancialAccountType(Guid userId, string financialAccountType);
        FinancialAccount Add(FinancialAccount financialAccount);
        FinancialAccount? GetById(Guid userId, Guid id);
    }
}