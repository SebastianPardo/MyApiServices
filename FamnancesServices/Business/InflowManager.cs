using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class InflowManager : IInflowManager
    {
        DatabaseContext _context;
        public InflowManager(DatabaseContext context)
        {
            this._context = context;
        }

        public decimal GetTotalByPeriod(DateTime startDate, DateTime endDate, Guid userId)
        {
            endDate = endDate.Hour == 0 ? endDate.AddHours(23).AddMinutes(59).AddSeconds(59) : endDate;
            return _context.Inflow.Where(e => e.TransactionDate >= startDate && e.TransactionDate <= endDate && e.UserId == userId).Sum(e => e.Value);
        }

        public Inflow GetById(Guid id)
        {
            return _context.Inflow.Single(e => e.Id == id);
        }

        public List<Inflow> GetAllByPeriod(DateTime startDate, DateTime endDate, Guid userId)
        {
            endDate = endDate.Hour == 0 ? endDate.AddHours(23).AddMinutes(59).AddSeconds(59) : endDate;
            return _context.Inflow.Where(e => e.TransactionDate >= startDate && e.TransactionDate <= endDate && e.UserId == userId).OrderByDescending(e => e.TransactionDate).ToList();
        }

        public List<Inflow?> GetByDiscountId(Guid id) => _context.InflowByDiscount.Where(e => e.IncomeDiscountId == id).Select(e => e.Inflow).ToList();
        public Inflow Add(Inflow inflow)
        {
            inflow.DateTimeStamp = DateTimeEast.Now;
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
            return _context.SaveChanges() > 0;
        }
    }
}
