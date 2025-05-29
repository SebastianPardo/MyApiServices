using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class AutomaticDiscountManager : IAutomaticDiscountManager
    {
        DatabaseContext context;
        public AutomaticDiscountManager(DatabaseContext context)
        {
            this.context = context;
        }

        public bool Add(AutomaticDiscounts entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(AutomaticDiscounts entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AutomaticDiscounts> GetAll()
        {
            throw new NotImplementedException();
        }

        public AutomaticDiscounts GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public AutomaticDiscounts Update(AutomaticDiscounts entity)
        {
            throw new NotImplementedException();
        }
    }
}
