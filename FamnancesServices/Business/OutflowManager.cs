using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class OutflowManager : IOutflowManager
    {
        DatabaseContext _context;
        public OutflowManager(DatabaseContext context)
        {
            this._context = context;
        }

        public Outflow Add(Outflow outflow)
        {
            outflow.DateTimeStamp = DateTime.Now;
            outflow = _context.Outflow.Add(outflow).Entity;
            _context.SaveChanges();
            return outflow;
        }

        public bool Delete(Outflow outflow)
        {
            _context.Outflow.Remove(outflow);
            return _context.SaveChanges() > 0;
        }

        public bool Update(Outflow outflow)
        {
            _context.Outflow.Update(outflow);
            _context.SaveChanges();
            return _context.SaveChanges() > 0;
        }

        public IEnumerable<Outflow> GetAllByUserId(Guid userId)
        {
            return _context.Outflow.Where(fe => fe.ExpensesBudget.UserId == userId);
        }

        public Outflow GetById(Guid id)
        {
            return _context.Outflow.FirstOrDefault(x => x.Id == id);
        }

        public decimal GetByPeriod(DateTime startDate, DateTime endDate, Guid userId)
        {
            return _context.Outflow.Where(e => e.DateTimeStamp >= startDate && e.DateTimeStamp <= endDate && e.ExpensesBudget.UserId == userId).Sum(e => e.Value);
        }
    }
}
