using AccountServices.Business.Interfaces;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;

namespace AccountServices.Business
{
    public class SavingsTransactionManager : ISavingsTransactionManager
    {
        DatabaseContext context;
        public SavingsTransactionManager(DatabaseContext context)
        {
            this.context = context;
        }

        public SavingsTransaction Add(SavingsTransaction savingsRecord)
        {
            savingsRecord = context.SavingsRecord.Add(savingsRecord).Entity;
            context.SaveChanges();
            return savingsRecord;
        }

        public bool Delete(SavingsTransaction savingsRecord)
        {
            context.SavingsRecord.Remove(savingsRecord);
            return context.SaveChanges() > 0;
        }

        public IEnumerable<SavingsTransaction> GetAllByUserId(Guid userId)
        {
            return context.SavingsRecord.Where(e => e.Id == userId);
        }

        public SavingsTransaction GetById(Guid id)
        {
            return context.SavingsRecord.FirstOrDefault(x => x.Id == id);
        }

        public bool Update(SavingsTransaction savingsRecord)
        {
            context.SavingsRecord.Update(savingsRecord);
            context.SaveChanges();
            return context.SaveChanges() > 0;
        }
    }
}
