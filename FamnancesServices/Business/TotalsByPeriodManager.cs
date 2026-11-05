using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Business
{
    public class TotalsByPeriodManager : ITotalsByPeriodManager
    {
        DatabaseContext _context;
        public TotalsByPeriodManager(DatabaseContext context)
        {
            _context = context;
        }

        public PeriodDates Save(PeriodDates PeriodDates)
        {
            PeriodDates = _context.PeriodsDates.Add(PeriodDates).Entity;
            _context.SaveChanges();
            return PeriodDates;

        }

        public PeriodDates? GetByDate(Guid userId, DateTime date)
        {
            return _context.PeriodsDates.Include(e => e.User).SingleOrDefault(e => e.UserId == userId && e.PeriodDateStart <= date && e.PeriodDateEnd >= date);
        }

        public PeriodDates? GetMostRecent(Guid userId)
        {
            return _context.PeriodsDates.OrderByDescending(e => e.PeriodDateStart).FirstOrDefault(e => e.UserId == userId);
        }

        public PeriodDates? GetByCurrentDay(Guid userId)
        {
            return _context.PeriodsDates.Include(e => e.User).SingleOrDefault(e=>e.UserId == userId && e.PeriodDateStart < DateTimeEast.Now && e.PeriodDateEnd > DateTimeEast.Now);
        }

        public bool Exist(Guid userId, DateTime date)
        {
            return _context.PeriodsDates.Any(e => e.UserId == userId && e.PeriodDateStart <= date && e.PeriodDateEnd >= date);
        }
    }
}
