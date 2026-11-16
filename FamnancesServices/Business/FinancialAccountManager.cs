using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Business
{
    public class FinancialAccountManager : IFinancialAccountManager
    {
        DatabaseContext context;
        public FinancialAccountManager(DatabaseContext context)
        {
            this.context = context;
        }

        public IEnumerable<FinancialAccount> GetByUserId(Guid userId) => context.FinancialAccount.Include(f => f.FinancialAccountType).Where(e => e.UserId == userId);

        public FinancialAccount? GetById(Guid userId, Guid id) => context.FinancialAccount.Include(f => f.FinancialAccountType).SingleOrDefault(e => e.UserId == userId && e.Id == id);
        public IEnumerable<FinancialAccount> GetByFinancialAccountType(Guid userId, string financialAccountType) 
            => context.FinancialAccount.Include(f => f.FinancialAccountType).Where(e => e.UserId == userId && e.FinancialAccountType.Code == financialAccountType);

        public FinancialAccount Add(FinancialAccount financialAccount)
        {
            financialAccount = context.FinancialAccount.Add(financialAccount).Entity;
            context.SaveChanges();
            return financialAccount;
        }
    }
}
