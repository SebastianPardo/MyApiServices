using AuthServices.Business.Interfaces;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthServices.Business
{
    public class AccountService : IAccountService
    {
        DatabaseContext context;
        public AccountService(DatabaseContext context)
        {
            this.context = context;
        }
        public Account? GetById(Guid id) => context.Account.Include(e => e.AccountType).FirstOrDefault(x => x.Id == id);
        public Account? getByUserNameOrEmail(string accountEmail) => context.Account.Include(e => e.User).FirstOrDefault(x => x.Email == accountEmail || x.UserName == accountEmail);

        public Account Add(Account account)
        {
            account.LastLogin = DateTime.Now;
            account = context.Account.Add(account).Entity;
            context.SaveChanges();
            return account;
        }
    }
}
