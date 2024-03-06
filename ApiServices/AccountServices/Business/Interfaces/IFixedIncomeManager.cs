using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business.Interfaces
{
    public interface IFixedIncomeManager
    {
        IEnumerable<FixedIncome> GetAllByUserId(Guid userId);
        FixedIncome GetById(Guid id);
        FixedIncome Add(FixedIncome fixedIncome);
        bool Update(FixedIncome fixedIncome);
        bool Delete(FixedIncome fixedIncome);
    }
}
