using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IFixedIncomeManager
    {
        IEnumerable<FixedIncome> GetAllByUserId(Guid userId);
        FixedIncome GetById(Guid userId, Guid id);
        FixedIncome Add(FixedIncome fixedIncome);
        bool Update(FixedIncome fixedIncome);
        bool Delete(FixedIncome fixedIncome);
    }
}
