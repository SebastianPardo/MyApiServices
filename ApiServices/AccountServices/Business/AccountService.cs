using AccountServices.Business.Interfaces;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountServices.Business
{
    public class AccountService : IAccountService
    {
        DatabaseContext context;
        public AccountService(DatabaseContext context)
        {
            this.context = context;
        }

        public IEnumerable<Account> GetAll() => context.Account;

        public Account? GetById(Guid id) => context.Account.FirstOrDefault(x => x.Id == id);
        public AccountType? GetType(Guid id) => context.Account.Include(e=>e.AccountType).FirstOrDefault(x => x.Id == id)?.AccountType;

        public Account? getByUserNameOrEmail(string accountEmail) => context.Account.Include(e => e.User).FirstOrDefault(x => x.Email == accountEmail || x.UserName == accountEmail);

        public Account Add(Account account)
        {
            account.LastLogin = DateTime.Now;
            account = context.Account.Add(account).Entity;
            context.SaveChanges();
            return account;
        }

        public bool Update(Account account)
        {
            context.Account.Update(account);
            return context.SaveChanges() > 0;
        }
        public bool Delete(Account account)
        {
            context.Account.Remove(account);
            return context.SaveChanges() > 0;
        }
    }
}
