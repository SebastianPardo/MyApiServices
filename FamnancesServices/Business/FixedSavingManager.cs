using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using FamnancesServices.Models;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Business
{
    public class FixedSavingManager : IFixedSavingManager
    {
        DatabaseContext _context;

        public FixedSavingManager(DatabaseContext context)
        {
            _context = context;
        }

        public FixedSaving Add(FixedSaving fixedSaving)
        {
            fixedSaving = _context.Add(fixedSaving).Entity;
            _context.SaveChanges();
            return fixedSaving;
        }

        public bool Delete(FixedSaving fixedSaving)
        {
            _context.Remove(fixedSaving);
            return _context.SaveChanges() > 0;
        }

        public IEnumerable<FixedSaving> GetAllByUserId(Guid userId)
        {
            return _context.FixedSaving.Include(e => e.SavingsPocket).Include(e=>e.SavingSource).Include(e => e.Periodicity).Where(e => e.SavingsPocket.UserId == userId);
        }

        public FixedSaving GetById(Guid userId, Guid id)
        {
            return _context.FixedSaving.Include(e => e.SavingsPocket).Include(e => e.SavingSource).Single(e => e.Id == id && e.SavingsPocket.UserId == userId);
        }

        public List<SummaryFixedSavingsModel> Summary(Guid userId, Guid? homeId, DateTime dateTime)
        {
            var totalByPeriod = _context.TotalsByPeriod.Single(e => dateTime >= e.PeriodDateStart && dateTime <= e.PeriodDateEnd && e.UserId == userId);
            return _context.FixedSaving.Where(e => (e.SavingsPocket.UserId == userId || (e.SavingsPocket.ShareOnHousehold && e.SavingsPocket.User.HomeId == homeId)) && e.IsActive == true)
                        .Select(e => new SummaryFixedSavingsModel
                        {
                            Id = e.Id,
                            Name = e.SavingsPocket.Name,
                            Value = e.Value,
                            WasTransferred =  (e.LastTransactionDate >= totalByPeriod.PeriodDateStart && e.LastTransactionDate <= totalByPeriod.PeriodDateEnd) || e.LastTransactionDate >= totalByPeriod.PeriodDateEnd,
                            UserId = e.SavingsPocket.UserId,
                            UserName = e.SavingsPocket.User.FirstName
                        }).ToList();
        }

        public bool Update(FixedSaving fixedSaving)
        {
            _context.FixedSaving.Update(fixedSaving);
            return _context.SaveChanges() > 0;
        }
    }
}
