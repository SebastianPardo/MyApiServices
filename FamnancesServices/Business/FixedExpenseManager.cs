using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Business
{
    public class FixedExpenseManager : IFixedExpenseManager
    {
        DatabaseContext context;
        public FixedExpenseManager(DatabaseContext context)
        {
            this.context = context;
        }

        public FixedExpense Add(FixedExpense fixedExpense)
        {
            fixedExpense = context.FixedExpense.Add(fixedExpense).Entity;
            context.SaveChanges();
            return fixedExpense;
        }

        public bool Delete(FixedExpense fixedExpense)
        {
            context.FixedExpense.Remove(fixedExpense);
            return context.SaveChanges() > 0;
        }

        public IEnumerable<FixedExpense> GetAllByHome(Guid id)
        {
            return context.FixedExpense.Where(e => e.User.HomeId == id);
        }

        public IEnumerable<FixedExpense> GetAllByUserId(Guid userId)
        {
            return context.FixedExpense.Include(e => e.FixedExpensesPaymentsRecord).Include(e => e.Period).Where(fe => fe.UserId == userId);
        }

        public FixedExpense? GetById(Guid id)
        {
            return context.FixedExpense.Include(e => e.FixedExpensesPaymentsRecord).FirstOrDefault(x => x.Id == id);
        }

        public FixedExpense? GetCompleteByIdDates(Guid id, DateTime from, DateTime to)
        {
            return context.FixedExpense.Include(e => e.FixedExpensesPaymentsRecord.Where(e => e.PaymentDate >= from && e.PaymentDate <= to).OrderBy(e=>e.PaymentDate))
                .FirstOrDefault(x => x.Id == id);
        }

        public bool Update(FixedExpense fixedExpense)
        {
            context.FixedExpense.Update(fixedExpense);
            return context.SaveChanges() > 0;
        }
    }
}
