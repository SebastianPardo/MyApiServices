using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
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
            savingsRecord.DateTimeStamp = DateTimeEast.Now;
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
            return _context.SavingRecord.Include(e=>e.SavingsPocket).Where(e => e.TransactionDate >= from && e.TransactionDate <= to && e.SavingsPocket.UserId == userId).OrderByDescending(e => e.TransactionDate);
        }

        public SavingRecord GetById(Guid id)
        {
            return _context.SavingRecord.Include(e => e.SavingsPocket).FirstOrDefault(x => x.Id == id);
        }

        public decimal GetSavingsExpensesByPeriod(DateTime startDate, DateTime endDate, Guid userId)
        {
            return _context.SavingRecord.Where(e => e.TransactionDate >= startDate && e.TransactionDate <= endDate && e.IsExpense == true && e.SavingsPocket.UserId == userId).Sum(e => e.Value);
        }

        public decimal GetSavingsIncomeByPeriod(DateTime startDate, DateTime endDate, Guid userId)
        {
            return _context.SavingRecord.Where(e => e.TransactionDate >= startDate && e.TransactionDate <= endDate && e.IsExpense == false && e.SavingsPocket.UserId == userId).Sum(e => e.Value);
        }

        public bool Update(SavingRecord savingsRecord)
        {
            try
            {
                savingsRecord.DateTimeStamp = DateTimeEast.Now;
                _context.SavingRecord.Update(savingsRecord);
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void ClosePeriod(List<RemainderBalance> remainderBalances)
        {
            foreach (var movement in remainderBalances) {
                if (movement.IsSavingPocket)
                {
                    var expensesBudgetPeriod = _context.ExpenseBudgetByPeriod.Include(e=>e.TotalsByPeriod).Single(e=>e.Id == movement.BudgetBalanceId);
                    var expenseBudget = _context.ExpensesBudget.Single(e => e.Id == expensesBudgetPeriod.ExpensesBudgetId);
                    var savingPocket = _context.SavingsPocket.Single(e => e.Id == movement.MoveToId);

                    var outflow = new Outflow
                    {
                        Description = $"Moving reminders from {expenseBudget.Name} To {savingPocket.Name}",
                        ExpenseBudgetId = expensesBudgetPeriod.ExpensesBudgetId,
                        Value = expensesBudgetPeriod.Budget - expensesBudgetPeriod.Expense,
                        TransactionDate = expensesBudgetPeriod.TotalsByPeriod.PeriodDateEnd.AddDays(-1),
                        DateTimeStamp = DateTimeEast.Now
                    };
                    _context.Outflow.Add(outflow);

                    var savingTransaction = new SavingRecord
                    {
                        Description = $"Moving reminders from {expenseBudget.Name} To {savingPocket.Name}",
                        IsEmergency = false,
                        SavingsPocketId = savingPocket.Id,
                        IsExpense = false,
                        Value = outflow.Value,
                        TransactionDate = expensesBudgetPeriod.TotalsByPeriod.PeriodDateEnd.AddDays(-1)
                    };
                    _context.SavingRecord.Add(savingTransaction);
                    _context.SaveChanges();
                }
            }
        }

        public IEnumerable<SavingRecord> GetByPocketId(Guid id)
        {
            return _context.SavingRecord.Where(e => e.SavingsPocketId == id);
        }
    }
}
