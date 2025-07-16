using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class SavingRecordManager : ISavingRecordManager
    {
        DatabaseContext _context;
        public SavingRecordManager(DatabaseContext context)
        {
            this._context = context;
        }

        public SavingRecord Add(SavingRecord savingsRecord)
        {
            savingsRecord = _context.SavingRecord.Add(savingsRecord).Entity;
            _context.SaveChanges();
            return savingsRecord;
        }

        public bool Delete(SavingRecord savingsRecord)
        {
            _context.SavingRecord.Remove(savingsRecord);
            return _context.SaveChanges() > 0;
        }

        public IEnumerable<SavingRecord> GetAllByUserId(Guid userId)
        {
            return _context.SavingRecord.Where(e => e.Id == userId);
        }

        public SavingRecord GetById(Guid id)
        {
            return _context.SavingRecord.FirstOrDefault(x => x.Id == id);
        }

        public decimal GetSavingsExpensesByPeriod(DateTime startDate, DateTime endDate)
        {
            return _context.SavingRecord.Where(e => e.TimeStamp >= startDate && e.TimeStamp <= endDate && e.IsExpense == true).Sum(e => e.Value);
        }

        public decimal GetSavingsIncomeByPeriod(DateTime startDate, DateTime endDate)
        {
            return _context.SavingRecord.Where(e => e.TimeStamp >= startDate && e.TimeStamp <= endDate && e.IsExpense == false).Sum(e => e.Value);
        }

        public bool Update(SavingRecord savingsRecord)
        {
            _context.SavingRecord.Update(savingsRecord);
            _context.SaveChanges();
            return _context.SaveChanges() > 0;
        }
    }
}
