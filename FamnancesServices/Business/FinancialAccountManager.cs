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

        public IEnumerable<FinancialAccount> GetAll(Guid userId) => context.FinancialAccount.Include(f => f.FinancialAccountType).Where(e => e.UserId == userId);

        public IEnumerable<FinancialAccount> GetByType(Guid userId, string code) => context.FinancialAccount.Include(f => f.FinancialAccountType).Where(e => e.UserId == userId && e.FinancialAccountType.Code == code);
    }
}
