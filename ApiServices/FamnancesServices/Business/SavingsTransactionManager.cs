using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
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
            savingsRecord = context.SavingsTransaction.Add(savingsRecord).Entity;
            context.SaveChanges();
            return savingsRecord;
        }

        public bool Delete(SavingsTransaction savingsRecord)
        {
            context.SavingsTransaction.Remove(savingsRecord);
            return context.SaveChanges() > 0;
        }

        public IEnumerable<SavingsTransaction> GetAllByUserId(Guid userId)
        {
            return context.SavingsTransaction.Where(e => e.Id == userId);
        }

        public SavingsTransaction GetById(Guid id)
        {
            return context.SavingsTransaction.FirstOrDefault(x => x.Id == id);
        }

        public bool Update(SavingsTransaction savingsRecord)
        {
            context.SavingsTransaction.Update(savingsRecord);
            context.SaveChanges();
            return context.SaveChanges() > 0;
        }
    }
}
