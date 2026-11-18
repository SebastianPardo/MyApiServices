using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Business
{
    public class FixedIncomeManager : IFixedIncomeManager
    {
        DatabaseContext context;
        public FixedIncomeManager(DatabaseContext context)
        {
            this.context = context;
        }

        public FixedIncome Add(FixedIncome fixedIncome)
        {
            if (fixedIncome.FixedIncomeByDiscount != null)
            {
                foreach (var byDiscount in fixedIncome.FixedIncomeByDiscount)
                {
                    byDiscount.FixedIncomeId = fixedIncome.Id;
                }
            }
            fixedIncome = context.FixedIncome.Add(fixedIncome).Entity;
            context.SaveChanges();
            return fixedIncome;
        }

        public bool Delete(FixedIncome fixedIncome)
        {
            context.FixedIncome.Remove(fixedIncome);
            return context.SaveChanges() > 0;
        }

        public IEnumerable<FixedIncome> GetAllByUserId(Guid userId)
        {
            return context.FixedIncome.Include(e => e.PayablePeriod).Where(e => e.FinancialAccount.UserId == userId);
        }

        public FixedIncome GetById(Guid userId, Guid id)
        {
            return context.FixedIncome.Include(e => e.FixedIncomeByDiscount).FirstOrDefault(x => x.Id == id && x.FinancialAccount.UserId == userId);
        }

        public void SetLastAutomaticTransaction (Guid fixedIncomeId)
        {
            var entity = context.FixedIncome.Single(e => e.Id == fixedIncomeId);
            entity.LastAutomaticDateStamp = DateTimeEast.Now;
            context.SaveChanges();
        }

        public bool Update(FixedIncome fixedIncome)
        {
            if (fixedIncome.FixedIncomeByDiscount != null)
            {
                foreach (var byDiscount in fixedIncome.FixedIncomeByDiscount)
                {
                    byDiscount.FixedIncomeId = fixedIncome.Id;
                }
            }
            context.FixedIncome.Update(fixedIncome);
            return context.SaveChanges() > 0;
        }
    }
}
