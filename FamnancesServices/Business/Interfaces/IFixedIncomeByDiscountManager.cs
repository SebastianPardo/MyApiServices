using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IFixedIncomeByDiscountManager
    {
        IEnumerable<FixedIncomeByDiscount> GetAll();
        FixedIncomeByDiscount GetById(Guid id);
        bool Add(FixedIncomeByDiscount entity);
        FixedIncomeByDiscount Update(FixedIncomeByDiscount entity);
        bool Delete(FixedIncomeByDiscount entity);

    }
}
