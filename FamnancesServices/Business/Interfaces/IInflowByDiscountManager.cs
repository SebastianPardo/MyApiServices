using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IInflowByDiscountManager
    {
        IEnumerable<InflowByDiscount> GetAll();
        InflowByDiscount GetById(Guid id);
        bool Add(InflowByDiscount entity);
        InflowByDiscount Update(InflowByDiscount entity);
        bool Delete(InflowByDiscount entity);

    }
}
