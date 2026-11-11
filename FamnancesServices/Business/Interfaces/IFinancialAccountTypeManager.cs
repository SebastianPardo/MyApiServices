using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IFinancialAccountTypeManager
    {
        IEnumerable<FinancialAccountType> GetAll();
        FinancialAccountType GetByCode(string code);
        FinancialAccountType GetById(Guid id);
    }
}