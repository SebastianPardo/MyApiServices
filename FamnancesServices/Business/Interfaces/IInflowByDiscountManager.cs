using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IInflowByDiscountManager
    {
        IEnumerable<InflowByDiscount> GetAll();
        InflowByDiscount GetById(Guid id);
        List<InflowByDiscount> GetDiscountsByInflow(Guid inflowId);
        bool Add(InflowByDiscount entity);
        InflowByDiscount Update(InflowByDiscount entity);
        bool Delete(InflowByDiscount entity);

    }
}
