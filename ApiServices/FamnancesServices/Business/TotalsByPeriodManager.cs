using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class TotalsByPeriodManager : ITotalsByPeriodManager
    {
        DatabaseContext _context;
        public TotalsByPeriodManager(DatabaseContext context)
        {
            _context = context;
        }
        public TotalsByPeriod? GetByCurrentPeriod()
        {
            return _context.TotalsByPeriod.SingleOrDefault(e => e.PeriodDateStart <= DateTime.Now && e.PeriodDateEnd >= DateTime.Now);
        }
    }
}
