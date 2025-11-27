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

        public decimal GetHomeSavings(Guid homeId)
        {
            var savingsPockets = _context.SavingsPocket.Include(e => e.User)
                .Where(e => e.ShareOnHousehold == true && e.User.HomeId == homeId);
            return savingsPockets.Sum(e => e.Total);
        }

        public SavingRecord Add(SavingRecord savingsRecord)
        {
            savingsRecord.DateTimeStamp = DateTime.Now;
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
            return _context.SavingRecord.Include(e => e.SavingsPocket).Where(e => e.SavingsPocket.UserId == userId).OrderByDescending(e => e.TransactionDate);
        }

        public IEnumerable<SavingRecord> GetAllByPeriod(DateTime from, DateTime to, Guid userId)
        {
            return _context.SavingRecord.Where(e => e.DateTimeStamp >= from && e.DateTimeStamp <= to && e.SavingsPocket.UserId == userId).OrderByDescending(e => e.TransactionDate);
        }

        public SavingRecord GetById(Guid id)
        {
            return _context.SavingRecord.Include(e => e.SavingsPocket).FirstOrDefault(x => x.Id == id);
        }

        public decimal GetSavingsExpensesByPeriod(DateTime startDate, DateTime endDate, Guid userId)
        {
            return _context.SavingRecord.Where(e => e.DateTimeStamp >= startDate && e.DateTimeStamp <= endDate && e.IsExpense == true && e.SavingsPocket.UserId == userId).Sum(e => e.Value);
        }

        public decimal GetSavingsIncomeByPeriod(DateTime startDate, DateTime endDate, Guid userId)
        {
            return _context.SavingRecord.Where(e => e.DateTimeStamp >= startDate && e.DateTimeStamp <= endDate && e.IsExpense == false && e.SavingsPocket.UserId == userId).Sum(e => e.Value);
        }

        public bool Update(SavingRecord savingsRecord)
        {
            try
            {
                savingsRecord.DateTimeStamp = DateTime.Now;
                _context.SavingRecord.Update(savingsRecord);
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
