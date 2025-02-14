using Famnances.DataCore.Entities;

namespace AccountServices.Business.Interfaces
{
    public interface IAccountManager
    {
        IEnumerable<Account> GetAll();
        Account GetById(Guid id);
        Account? getByUserNameOrEmail(string accountEmail);
        Account Add(Account account);
        bool Update(Account account);
        bool Delete(Account account);

    }
}
