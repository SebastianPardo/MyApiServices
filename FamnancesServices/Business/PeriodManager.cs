using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class PeriodManager : IPeriodManager
    {
        DatabaseContext _context;
        public PeriodManager(DatabaseContext context)
        {
            _context = context;
        }
        public IEnumerable<Period> GetAll()
        {
            return _context.Period;
        }

        public Period GetByCode(string code)
        {
            return _context.Period.Single(e => e.Code == code);
        }
    }
}
