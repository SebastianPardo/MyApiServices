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
    }
}
