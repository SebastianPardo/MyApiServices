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

        public TotalsByPeriod? GetByCurrentPeriod(Guid userId)
        {
            try
            {
                return _context.TotalsByPeriod.Include(e => e.User).SingleOrDefault(e => e.UserId == userId && e.PeriodDateStart <= DateTime.Now && e.PeriodDateEnd >= DateTime.Now);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public TotalsByPeriod? GetByCurrentDay(Guid userId)
        {
            return _context.TotalsByPeriod.SingleOrDefault(e=>e.UserId == userId && e.PeriodDateStart < DateTime.Now && e.PeriodDateEnd > DateTime.Now);
        }
    }
}
