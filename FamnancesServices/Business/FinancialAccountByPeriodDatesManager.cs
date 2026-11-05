using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Business
{
    public class FinancialAccountByPeriodDatesManager : IFinancialAccountByPeriodDatesManager
    {
        DatabaseContext context;
        public FinancialAccountByPeriodDatesManager(DatabaseContext context)
        {
            this.context = context;
        }

        public IEnumerable<FinancialAccountByPeriodDates> GetByUserId(Guid userId) => context.FinancialAccountByPeriodDates.Include(f => f.FinancialAccount).ThenInclude(f => f.FinancialAccountType).Where(e => e.FinancialAccount.UserId == userId);
    }
}
