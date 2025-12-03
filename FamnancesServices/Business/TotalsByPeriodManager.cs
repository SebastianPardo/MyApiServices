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

        public TotalsByPeriod Save(TotalsByPeriod totalsByPeriod)
        {
            totalsByPeriod = _context.TotalsByPeriod.Add(totalsByPeriod).Entity;
            _context.SaveChanges();
            return totalsByPeriod;

        }

        public TotalsByPeriod? GetByDate(Guid userId, DateTime date)
        {
            return _context.TotalsByPeriod.Include(e => e.User).SingleOrDefault(e => e.UserId == userId && e.PeriodDateStart <= date && e.PeriodDateEnd >= date);
        }

        public TotalsByPeriod? GetByCurrentDay(Guid userId)
        {
            return _context.TotalsByPeriod.Include(e => e.User).SingleOrDefault(e=>e.UserId == userId && e.PeriodDateStart < DateTimeEast.Now && e.PeriodDateEnd > DateTimeEast.Now);
        }
    }
}
