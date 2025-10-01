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

        public bool Delete(FixedIncomeByDiscount entity)
        {
            throw new NotImplementedException();
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
