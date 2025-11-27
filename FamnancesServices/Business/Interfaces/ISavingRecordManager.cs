using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface ISavingRecordManager
    {
        IEnumerable<SavingRecord> GetAll(Guid userId);
        IEnumerable<SavingRecord> GetAllByPeriod(DateTime item1, DateTime item2, Guid userId);
        SavingRecord GetById(Guid id);
        SavingRecord Add(SavingRecord savingsRecord);
        bool Update(SavingRecord savingsRecord);
        bool Delete(SavingRecord savingsRecord);
        decimal GetSavingsIncomeByPeriod(DateTime item1, DateTime item2, Guid userId);
        decimal GetSavingsExpensesByPeriod(DateTime item1, DateTime item2, Guid userId);
        decimal GetHomeSavings(Guid homeId);
    }
}
