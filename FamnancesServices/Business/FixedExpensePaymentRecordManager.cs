using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Business
{
    public class FixedExpensePaymentRecordManager : IFixedExpensePaymentRecordManager
    {
        DatabaseContext context;
        public FixedExpensePaymentRecordManager(DatabaseContext context)
        {
            this.context = context;
        }

        public FixedExpensePaymentRecord Add(FixedExpensePaymentRecord record)
        {
            record = context.FixedExpensePaymentRecord.Add(record).Entity;
            context.SaveChanges();
            return record;
        }

        public bool Delete(FixedExpensePaymentRecord record)
        {
            context.FixedExpensePaymentRecord.Remove(record);
            return context.SaveChanges() > 0;
        }

        public FixedExpensePaymentRecord GetById(Guid id)
        {
            return context.FixedExpensePaymentRecord.Include(e => e.FixedExpense).FirstOrDefault(x => x.Id == id);
        }

        public bool Update(FixedExpensePaymentRecord record)
        {
            context.FixedExpensePaymentRecord.Update(record);
            return context.SaveChanges() > 0;
        }
    }
}
