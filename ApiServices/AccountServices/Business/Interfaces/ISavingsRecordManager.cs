using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business.Interfaces
{
    public interface ISavingsRecordManager
    {
        IEnumerable<SavingsRecord> GetAllByUserId(Guid userId);
        SavingsRecord GetById(Guid id);
        SavingsRecord Add(SavingsRecord savingsRecord);
        bool Update(SavingsRecord savingsRecord);
        bool Delete(SavingsRecord savingsRecord);
    }
}
