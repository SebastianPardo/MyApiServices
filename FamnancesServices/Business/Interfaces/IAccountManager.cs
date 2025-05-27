using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IAccountManager
    {
        Account? getByUserNameOrEmail(string accountEmail);
        IEnumerable<Account> GetAll();
        Account GetById(Guid id);
        bool Add(Account account);
        Account Update(Account account);
        bool Delete(Account account);

    }
}
