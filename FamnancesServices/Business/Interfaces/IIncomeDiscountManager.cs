using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IIncomeDiscountManager
    {
        IncomeDiscount Add(IncomeDiscount incomeDiscount);
        IEnumerable<IncomeDiscount> GetAllByUser(Guid userId);
        IncomeDiscount? GetById(Guid id);
        bool Update(IncomeDiscount incomeDiscount);
        bool Delete(IncomeDiscount entity);
    }
}