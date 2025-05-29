using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IAutomaticDiscountManager
    {
        IEnumerable<AutomaticDiscounts> GetAll();
        AutomaticDiscounts GetById(Guid id);
        bool Add(AutomaticDiscounts entity);
        AutomaticDiscounts Update(AutomaticDiscounts entity);
        bool Delete(AutomaticDiscounts entity);

    }
}
