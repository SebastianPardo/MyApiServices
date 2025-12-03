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

        private List<SavingsPocket> SavingsPockets(Guid userId, DateTime from, DateTime to, bool guest)
        {
            return context.SavingsPocket.Where(e => e.UserId == userId && (e.ShareOnHousehold == true || e.ShareOnHousehold == guest))
                                    .Select(e => new SavingsPocket
                                    {
                                        Id = guest ? Guid.Empty : e.Id,
                                        Name = e.Name,
                                        SavingsRecords = e.SavingsRecords.Where(ee => ee.TransactionDate >= from && ee.TransactionDate <= to).ToList(),
                                        Total = e.Total,
                                    }).ToList();
        }

        private List<ExpensesBudget> ExpensesBudgets(Guid userId, DateTime from, DateTime to, bool guest)
        {
            return context.ExpensesBudget.Where(e => e.UserId == userId && (e.ShareOnHousehold == true || e.ShareOnHousehold == guest))
                                    .Select(e => new ExpensesBudget
                                    {
                                        Id = guest ? Guid.Empty : e.Id,
                                        Name = e.Name,
                                        Value = e.Value,
                                        Outflow = e.Outflow.Where(ee => ee.TransactionDate >= from && ee.TransactionDate <= to).ToList()
                                    }).ToList();
        }

        private List<FixedExpense> FixedExpenses(Guid userId, DateTime from, DateTime to, bool guest)
        {
            return context.FixedExpense.Where(e => e.UserId == userId && (e.ShareOnHousehold == true || e.ShareOnHousehold == guest))
                                .Select(e => new FixedExpense
                                {
                                    Id = guest ? Guid.Empty : e.Id,
                                    Name = e.Name,
                                    Value = e.Value,
                                    FixedExpensesPaymentsRecord = e.FixedExpensesPaymentsRecord.Where(ee => ee.PaymentDate >= from && ee.PaymentDate <= to).ToList(),
                                }).ToList();
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
                    ExpensesBudget = home.ShareSavings ? ExpensesBudgets(e.Id, totalsByPeriod.PeriodDateStart, totalsByPeriod.PeriodDateEnd, e.Id != userId) : null,
                    SavingsPockets = home.ShareSavings ? SavingsPockets(e.Id, totalsByPeriod.PeriodDateStart, totalsByPeriod.PeriodDateEnd, e.Id != userId) : null,
                    FixedExpense = home.ShareSavings ? FixedExpenses(e.Id, totalsByPeriod.PeriodDateStart, totalsByPeriod.PeriodDateEnd, e.Id != userId) : null
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