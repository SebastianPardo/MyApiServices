using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class FixedIncomeByDiscountManager : IFixedIncomeByDiscountManager
    {
        DatabaseContext context;
        public FixedIncomeByDiscountManager(DatabaseContext context)
        {
            this.context = context;
        }

        public bool Add(FixedIncomeByDiscount entity)
        {
            throw new NotImplementedException();
        }

        public bool DeleteByFixedIncomme(Guid fixedIncommeId)
        {
            var fixedIncomesByDiscounts = context.FixedIncomeByDiscount.Where(e => e.FixedIncomeId == fixedIncommeId);
            context.FixedIncomeByDiscount.RemoveRange(fixedIncomesByDiscounts);
            return context.SaveChanges() > 0;
        }

        public IEnumerable<FixedIncomeByDiscount> GetAll()
        {
            throw new NotImplementedException();
        }

        public FixedIncomeByDiscount GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public FixedIncomeByDiscount Update(FixedIncomeByDiscount entity)
        {
            throw new NotImplementedException();
        }
    }
}
