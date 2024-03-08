using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business.Interfaces
{
    public interface ISavingsPocketManager
    {
        IEnumerable<SavingsPocket> GetAllByUserId(Guid userId);
        SavingsPocket GetById(Guid id);
        SavingsPocket Add(SavingsPocket savingsPocket);
        bool Update(SavingsPocket savingsPocket);
        bool Delete(SavingsPocket savingsPocket);
    }
}
