using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IAccountTypeManager
    {
        IEnumerable<AccountType> GetAll();
        AccountType GetById(Guid id);
        bool Add(AccountType entity);
        AccountType Update(AccountType entity);
        bool Delete(AccountType entity);

    }
}
