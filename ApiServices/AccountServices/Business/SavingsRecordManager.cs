using AccountServices.Business.Interfaces;
using OhMyMoney.DataCore.Data;
using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business
{
    public class SavingsRecordManager : ISavingsRecordManager
    {
        DatabaseContext context;
        public SavingsRecordManager(DatabaseContext context)
        {
            this.context = context;
        }

        public SavingsRecord Add(SavingsRecord savingsRecord)
        {
            savingsRecord = context.SavingsRecord.Add(savingsRecord).Entity;
            context.SaveChanges();
            return savingsRecord;
        }

        public bool Delete(SavingsRecord savingsRecord)
        {
            context.SavingsRecord.Remove(savingsRecord);
            return context.SaveChanges() > 0;
        }

        public IEnumerable<SavingsRecord> GetAllByUserId(Guid userId)
        {
            return context.SavingsRecord.Where(e => e.Id == userId);
        }

        public SavingsRecord GetById(Guid id)
        {
            return context.SavingsRecord.FirstOrDefault(x => x.Id == id);
        }

        public bool Update(SavingsRecord savingsRecord)
        {
            context.SavingsRecord.Update(savingsRecord);
            context.SaveChanges();
            return context.SaveChanges() > 0;
        }
    }
}
