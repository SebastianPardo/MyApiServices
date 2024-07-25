using Famnances.DataCore.Entities;

namespace AccountServices.Business.Interfaces
{
    public interface ISavingsTransactionManager
    {
        IEnumerable<SavingsTransaction> GetAllByUserId(Guid userId);
        SavingsTransaction GetById(Guid id);
        SavingsTransaction Add(SavingsTransaction savingsRecord);
        bool Update(SavingsTransaction savingsRecord);
        bool Delete(SavingsTransaction savingsRecord);
    }
}
