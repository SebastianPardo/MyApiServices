using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IIncomeDiscountManager
    {
        IncomeDiscount Add(IncomeDiscount incomeDiscount);
        IEnumerable<IncomeDiscount> GetAllByUser(Guid userId);
        bool Update(IncomeDiscount incomeDiscount);
    }
}