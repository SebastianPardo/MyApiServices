using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class SavingRecordManager : ISavingRecordManager
    {
        DatabaseContext context;
        public SavingRecordManager(DatabaseContext context)
        {
            this.context = context;
        }

        public SavingRecord Add(SavingRecord savingsRecord)
        {
            savingsRecord = context.SavingRecord.Add(savingsRecord).Entity;
            context.SaveChanges();
            return savingsRecord;
        }

        public bool Delete(SavingRecord savingsRecord)
        {
            context.SavingRecord.Remove(savingsRecord);
            return context.SaveChanges() > 0;
        }

        public IEnumerable<SavingRecord> GetAllByUserId(Guid userId)
        {
            return context.SavingRecord.Where(e => e.Id == userId);
        }

        public SavingRecord GetById(Guid id)
        {
            return context.SavingRecord.FirstOrDefault(x => x.Id == id);
        }

        public bool Update(SavingRecord savingsRecord)
        {
            context.SavingRecord.Update(savingsRecord);
            context.SaveChanges();
            return context.SaveChanges() > 0;
        }
    }
}
