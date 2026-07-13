using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IIncomeDiscountManager
    {
        IEnumerable<IncomeDiscount> GetAllByUser(Guid userId);
        IncomeDiscount? GetById(Guid id);
        IncomeDiscount Add(IncomeDiscount incomeDiscount);
        bool Update(IncomeDiscount incomeDiscount);
        bool Delete(IncomeDiscount discount);
    }
}