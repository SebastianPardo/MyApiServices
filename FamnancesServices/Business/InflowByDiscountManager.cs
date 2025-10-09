using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class InflowByDiscountManager : IInflowByDiscountManager
    {
        DatabaseContext _context;
        public InflowByDiscountManager(DatabaseContext context)
        {
            _context = context;
        }

        public bool Add(InflowByDiscount entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(InflowByDiscount entity)
        {
            _context.InflowByDiscount.Remove(entity);
            return _context.SaveChanges() > 0;
        }

        public IEnumerable<InflowByDiscount> GetAll()
        {
            throw new NotImplementedException();
        }

        public InflowByDiscount GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public InflowByDiscount Update(InflowByDiscount entity)
        {
            throw new NotImplementedException();
        }
    }
}
