using Famnances.DataCore.Entities;

namespace AuthServices.Business.Interfaces
{
    public interface IAccountService
    {
        Account? GetById(Guid id);
        Account? getByUserNameOrEmail(string accountEmail);
        AccountType GetDefault();
        Account Add(Account account);
        void RegisterLogin(Guid accountId);
        bool Any(string userName);
    }
}
