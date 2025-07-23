using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Business
{
    public class InflowManager : IInflowManager
    {
        DatabaseContext _context;
        public InflowManager(DatabaseContext context)
        {
            this._context = context;
        }

        public Inflow Add(Inflow inflow)
        {
            inflow.DateTimeStamp = DateTime.Now;
            inflow = _context.Inflow.Add(inflow).Entity;
            _context.SaveChanges();
            return inflow;
        }

        public bool Delete(Inflow inflow)
        {
            _context.Inflow.Remove(inflow);
            return _context.SaveChanges() > 0;
        }

        public bool Update(Inflow inflow)
        {
            _context.Inflow.Update(inflow);
            _context.SaveChanges();
            return _context.SaveChanges() > 0;
        }
        public decimal GetTotalByPeriod(DateTime startDate, DateTime endDate, Guid userId)
        {
            return _context.Inflow.Where(e => e.DateTimeStamp >= startDate && e.DateTimeStamp <= endDate && e.UserId == userId).Sum(e => e.Value);
        }

        public Inflow GetById(Guid id)
        {
            return _context.Inflow.Single(e => e.Id == id);
        }

        public List<Inflow> GetAllByPeriod(DateTime startDate, DateTime endDate, Guid userId)
        {
            return _context.Inflow.Where(e => e.DateTimeStamp >= startDate && e.DateTimeStamp <= endDate && e.UserId == userId).ToList();
        }
    }
}
