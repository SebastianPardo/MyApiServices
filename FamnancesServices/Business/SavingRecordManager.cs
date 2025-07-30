using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            savingsRecord.TimeStamp = DateTime.Now;
            savingsRecord = _context.SavingRecord.Add(savingsRecord).Entity;
            _context.SaveChanges();
            return savingsRecord;
        }

        public bool Delete(SavingRecord savingsRecord)
        {
            _context.SavingRecord.Remove(savingsRecord);
            return _context.SaveChanges() > 0;
        }

        public IEnumerable<SavingRecord> GetAll(Guid userId)
        {
            return _context.SavingRecord.Include(e => e.SavingsPocket).Where(e => e.SavingsPocket.UserId == userId);
        }

        public SavingRecord GetById(Guid id)
        {
            return _context.SavingRecord.Include(e => e.SavingsPocket).FirstOrDefault(x => x.Id == id);
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
            savingsRecord.TimeStamp = DateTime.Now;
            _context.SavingRecord.Update(savingsRecord);
            _context.SaveChanges();
            return _context.SaveChanges() > 0;
        }
    }
}
