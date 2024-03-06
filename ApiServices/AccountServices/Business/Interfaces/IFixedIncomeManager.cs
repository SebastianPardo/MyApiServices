using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business.Interfaces
{
    public interface IFixedIncomeManager
    {
        IEnumerable<FixedIncome> GetAllByUserId(Guid userId);
        FixedIncome GetById(Guid id);
        FixedIncome Add(FixedIncome user);
        bool Update(FixedIncome user);
        bool Delete(FixedIncome user);
    }
}
