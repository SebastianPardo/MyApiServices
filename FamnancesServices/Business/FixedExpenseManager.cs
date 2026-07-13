using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using FamnancesServices.Models;
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

        public IEnumerable<FixedExpense> GetAllByHome(Guid id)
        {
            return context.FixedExpense.Where(e => e.User.HomeId == id);
        }

        public IEnumerable<FixedExpense> GetAllByUserId(Guid userId)
        {
            return context.FixedExpense.Include(e => e.FixedExpensesPaymentsRecord).Include(e => e.Period).Where(fe => fe.UserId == userId);
        }

        public FixedExpense? GetById(Guid userId, Guid id)
        {
            return context.FixedExpense.Include(e => e.FixedExpensesPaymentsRecord).FirstOrDefault(x => x.Id == id && x.UserId == userId);
        }

        public FixedExpense? GetCompleteByIdDates(Guid userId, Guid id, DateTime from, DateTime to)
        {
            return context.FixedExpense.Include(e => e.FixedExpensesPaymentsRecord.Where(e => e.PaymentDate >= from && e.PaymentDate <= to).OrderBy(e => e.PaymentDate))
                .FirstOrDefault(x => x.Id == id && x.UserId == userId);
        }

        public bool Update(FixedExpense fixedExpense)
        {
            context.FixedExpense.Update(fixedExpense);
            return context.SaveChanges() > 0;
        }

        public List<SummaryFixedExpensesModel> Summary(Guid userId, Guid? householdId, DateTime dateTime)
        {
            var totalByPeriod = context.TotalsByPeriod.Single(e => dateTime >= e.PeriodDateStart && dateTime <= e.PeriodDateEnd && e.UserId == userId);
            return context.FixedExpense.Where(e => e.UserId == userId || (e.ShareOnHousehold && e.User.HomeId == householdId))
                        .Select(e => new SummaryFixedExpensesModel
                        {
                            Id = e.Id,
                            Name = e.Name,
                            Value = e.Value,
                            WasPaid = e.FixedExpensesPaymentsRecord.Any(e => e.PaymentDate >= totalByPeriod.PeriodDateStart && e.PaymentDate <= totalByPeriod.PeriodDateEnd),
                            UserId = e.UserId,
                            UserName = e.User.FirstName
                        }).ToList();
        }

        public bool Delete(FixedExpense fixedExpense)
        {
            context.FixedExpense.Remove(fixedExpense);
            return context.SaveChanges() > 0;
        }
    }
}
