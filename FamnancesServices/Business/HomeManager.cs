using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace FamnancesServices.Business
{
    public class HomeManager : IHomeManager
    {
        DatabaseContext context;
        public HomeManager(DatabaseContext context)
        {
            this.context = context;
        }
        public Home? GetById(Guid id)
        {
            return context.Home.FirstOrDefault(u => u.Id == id);
        }

        public Home? GetByUser(Guid userId)
        {
            return context.Home.FirstOrDefault(e => e.Users.Any(ee => ee.Id == userId));
        }

        public Home GetComplete(Guid userId, DateTime date)
        {
            User user = context.User.First(u => u.Id == userId);
            var totalsByPeriod = context.TotalsByPeriod.SingleOrDefault(e => e.UserId == userId && e.PeriodDateStart <= date && e.PeriodDateEnd >= date);
            if (user.HomeId != null)
            {
                Home home = context.Home.Include(e => e.Users).First(e => e.Id == user.HomeId);
                home.Users = home.Users.Select(e => new User
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    HomeId = e.HomeId,
                    ExpensesBudget = e.Id != userId ?
                            home.ShareExpenses ? 
                                context.ExpensesBudget.Where(ee => ee.UserId == e.Id)
                                    .Include(ee => ee.Outflow
                                                .Where(eee => eee.TransactionDate >= totalsByPeriod.PeriodDateStart && eee.TransactionDate <= totalsByPeriod.PeriodDateEnd)
                                    ).ToList() : null
                            : context.ExpensesBudget.Where(ee => ee.UserId == userId)
                                    .Include(ee => ee.Outflow
                                                .Where(eee => eee.TransactionDate >= totalsByPeriod.PeriodDateStart && eee.TransactionDate <= totalsByPeriod.PeriodDateEnd)
                                    ).ToList(),
                    SavingsPockets = e.Id != userId ?
                        home.ShareSavings ? context.SavingsPocket.Where(ee => ee.UserId == e.Id)
                                    .Include(ee => ee.SavingsRecords
                                                .Where(eee => eee.TransactionDate >= totalsByPeriod.PeriodDateStart && eee.TransactionDate <= totalsByPeriod.PeriodDateEnd)
                                    ).ToList() : null
                        : context.SavingsPocket.Where(ee => ee.UserId == userId)
                                    .Include(ee => ee.SavingsRecords
                                                .Where(eee => eee.TransactionDate >= totalsByPeriod.PeriodDateStart && eee.TransactionDate <= totalsByPeriod.PeriodDateEnd)
                                    ).ToList(),
                    FixedExpense = e.Id != userId ?
                        home.ShareExpenses ?
                            context.FixedExpense.Where(ee => ee.UserId == e.Id).Select(eee => 
                            new FixedExpense {
                                Id = eee.Id,
                                Name = eee.Name,
                                Value = eee.Value,
                                FixedExpensesPaymentsRecord = context.FixedExpensePaymentRecord
                                                                .Where(eeee => eeee.FixedExpenseId == eee.Id 
                                                                    && eeee.PaymentDate >= totalsByPeriod.PeriodDateStart
                                                                    && eeee.PaymentDate <= totalsByPeriod.PeriodDateEnd).ToList(),
                            }).ToList() : null
                            : context.FixedExpense.Where(ee => ee.UserId == userId).Select(eee =>
                                new FixedExpense
                                {
                                    Id = eee.Id,
                                    Name = eee.Name,
                                    Value = eee.Value,
                                    UserId = eee.UserId,
                                    FixedExpensesPaymentsRecord = context.FixedExpensePaymentRecord
                                                                .Where(eeee => eeee.FixedExpenseId == eee.Id
                                                                    && eeee.PaymentDate >= totalsByPeriod.PeriodDateStart
                                                                    && eeee.PaymentDate <= totalsByPeriod.PeriodDateEnd).ToList(),
                                }).ToList(),
                }).ToList();
                return home;
            }
            return new Home();
        }

        public bool Add(Home home)
        {
            context.Home.Add(home);
            return context.SaveChanges() > 0;
        }

        public bool Delete(Home home)
        {
            context.Home.Remove(home);
            return context.SaveChanges() > 0;
        }

        public Home Update(Home home)
        {
            home = context.Home.Update(home).Entity;
            context.SaveChanges();
            return home;
        }
    }
}