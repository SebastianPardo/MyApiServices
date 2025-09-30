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

        public Home GetComplete(Guid userId)
        {
            User user = context.User.First(u => u.Id == userId);
            var totalsByPeriod = context.TotalsByPeriod.SingleOrDefault(e => e.UserId == userId && e.PeriodDateStart < DateTime.Now && e.PeriodDateEnd > DateTime.Now);
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
                            home.ShareExpenses ? context.ExpensesBudget.Include(ee => ee.Outflow)
                                                    .Where(ee => ee.UserId == e.Id
                                                            && ee.Outflow.Any(
                                                                eee => eee.TransactionDate >= totalsByPeriod.PeriodDateStart
                                                                && eee.TransactionDate <= totalsByPeriod.PeriodDateEnd
                                                    )).ToList() : null
                            : context.ExpensesBudget.Include(ee => ee.Outflow).Where(ee => ee.UserId == userId).ToList(),
                    SavingsPockets = e.Id != userId ?
                        home.ShareSavings ? context.SavingsPocket.Include(ee => ee.SavingsRecords)
                                                .Where(ee => ee.UserId == e.Id
                                                    && ee.SavingsRecords.Any(
                                                            eee => eee.TransactionDate >= totalsByPeriod.PeriodDateStart
                                                            && eee.TransactionDate <= totalsByPeriod.PeriodDateEnd
                                                )).ToList() : null
                        : context.SavingsPocket.Include(ee => ee.SavingsRecords).Where(ee => ee.UserId == userId).ToList(),
                    FixedExpense = e.Id != userId ?
                        home.ShareExpenses ?
                            context.FixedExpense.Where(ee => ee.UserId == e.Id).ToList().Select(eee => new FixedExpense
                            {
                                Id = eee.LastAutomaticDateStamp >= totalsByPeriod.PeriodDateStart && eee.LastAutomaticDateStamp <= totalsByPeriod.PeriodDateEnd ? Guid.Empty : eee.Id,
                                Name = eee.Name,
                                Value = eee.Value
                            }).ToList() : null
                            : context.FixedExpense.Where(ee => ee.UserId == userId).Select(eee => new FixedExpense
                            {
                                Id = eee.LastAutomaticDateStamp >= totalsByPeriod.PeriodDateStart && eee.LastAutomaticDateStamp <= totalsByPeriod.PeriodDateEnd ? Guid.Empty : eee.Id,
                                Name = eee.Name,
                                Value = eee.Value,
                                UserId = eee.UserId
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