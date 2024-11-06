using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface ISavingRecordManager
    {
        IEnumerable<SavingRecord> GetAllByUserId(Guid userId);
        SavingRecord GetById(Guid id);
        SavingRecord Add(SavingRecord savingsRecord);
        bool Update(SavingRecord savingsRecord);
        bool Delete(SavingRecord savingsRecord);
    }
}
