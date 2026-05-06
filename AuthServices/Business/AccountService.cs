using AuthServices.Business.Interfaces;
using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using Microsoft.EntityFrameworkCore;
using Famnances.Core.Security;

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
        public Account? getByUserNameOrEmail(string accountEmail)
        {
            try
            {
                return context.Account.Include(e => e.User).FirstOrDefault(x => x.Email == accountEmail || x.UserName == accountEmail);
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.InnerException?.Message == "No such host is known.")
                {
                    return new Account { UserName = Constants.NO_DATABASE };
                }
                return null;
            }
        }

        public bool Any(string userName)
        {
            return context.Account.Any(e => e.UserName == userName);
        }

        public AccountType GetDefault()
        {
            return context.AccountType.First(e=>e.Code == "REG");
        }

        public Account Add(Account account)
        {
            account.LastLogin = DateTimeEast.Now;
            account = context.Account.Add(account).Entity;
            context.SaveChanges();
            return account;
        }
    }
}
