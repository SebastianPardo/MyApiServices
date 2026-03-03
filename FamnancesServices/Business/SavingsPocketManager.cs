using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using FamnancesServices.Models;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Business
{
    public class SavingsPocketManager : ISavingsPocketManager
    {
        DatabaseContext context;
        public SavingsPocketManager(DatabaseContext context)
        {
            this.context = context;
        }


        public SavingsPocket Add(SavingsPocket savingsPocket)
        {
            savingsPocket = context.SavingsPocket.Add(savingsPocket).Entity;
            context.SaveChanges();
            return savingsPocket;
        }

        public bool Delete(SavingsPocket savingsPocket)
        {
            context.SavingsPocket.Remove(savingsPocket);
            return context.SaveChanges() > 0;
        }

        public IEnumerable<SavingsPocket> GetAllByUserId(Guid userId)
        {
            return context.SavingsPocket.Include(e => e.SavingsRecords).Where(e => e.UserId == userId);
        }

        public IEnumerable<SavingsPocket> GetAllByHome(Guid homeId)
        {
            return context.SavingsPocket.Include(e => e.SavingsRecords).Where(e => e.User.HomeId == homeId);
        }

        public SavingsPocket GetById(Guid id)
        {
            return context.SavingsPocket.FirstOrDefault(x => x.Id == id);
        }

        public bool Update(SavingsPocket savingsPocket)
        {
            context.SavingsPocket.Update(savingsPocket);
            return context.SaveChanges() > 0;
        }

        public SavingsPocket? GetCompleteByIdDates(Guid id, DateTime from, DateTime to)
        {
            return context.SavingsPocket.Include(e => e.SavingsRecords.Where(e => e.TransactionDate >= from && e.TransactionDate <= to).OrderBy(e => e.TransactionDate))
                .FirstOrDefault(x => x.Id == id);
        }

        public List<SummaryPocketModel> Summary(Guid userId, DateTime dateTime)
        {
            var totalByPeriod = context.TotalsByPeriod.Single(e => dateTime >= e.PeriodDateStart && dateTime <= e.PeriodDateEnd && e.UserId == userId);
            return context.SavingsPocket
                .Where(e => e.UserId == userId || e.ShareOnHousehold)
                        .Select(e => new SummaryPocketModel
                        {
                            Id = e.Id,
                            Name = e.Name,
                            Value = e.Total,
                            Spent = e.SavingsRecords.Where(ee => ee.IsExpense && ee.TransactionDate >= totalByPeriod.PeriodDateStart && ee.TransactionDate <= totalByPeriod.PeriodDateEnd).Sum(e => e.Value),
                            UserId = e.UserId,
                            UserName = e.User.FirstName
                        }).ToList();
        }
    }
}
