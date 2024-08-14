using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class AccountManager : IAccountManager
    {
        DatabaseContext context;
        public AccountManager(DatabaseContext context)
        {
            this.context = context;
        }

        public bool Add(Account account)
        {
            context.Account.Add(account);
            return context.SaveChanges() > 0;
        }

        public bool Delete(Account account)
        {
            context.Account.Remove(account);
            return context.SaveChanges() > 0;
        }

        public IEnumerable<Account> GetAll()
        {
            return context.Account;
        }

        public Account GetById(Guid id)
        {
            return context.Account.FirstOrDefault(x => x.Id == id);
        }

        public Account? getByUserNameOrEmail(string accountEmail)
        {
            return context.Account.FirstOrDefault(x => x.Email == accountEmail || x.UserName == accountEmail);
        }

        public Account Update(Account account)
        {
            account = context.Account.Update(account).Entity;
            context.SaveChanges();
            return account;
        }
    }
}
