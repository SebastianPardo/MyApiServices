using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class IncomeDiscountManager : IIncomeDiscountManager
    {
        DatabaseContext _context;
        public IncomeDiscountManager(DatabaseContext context)
        {
            _context = context;
        }
        public IncomeDiscount Add(IncomeDiscount entity)
        {
            entity = _context.IncomeDiscount.Add(entity).Entity;
            _context.SaveChanges();
            return entity;
        }

        public IEnumerable<IncomeDiscount> GetAllByUser(Guid userId)
        {
            return _context.IncomeDiscount.Where(e => e.UserId == userId);
        }

        public bool Update(IncomeDiscount entity)
        {
            _context.IncomeDiscount.Update(entity);
            return _context.SaveChanges() > 0;
        }

        public bool Delete(IncomeDiscount entity)
        {
            _context.IncomeDiscount.Remove(entity);
            return _context.SaveChanges() > 0;
        }

        public IncomeDiscount? GetById(Guid id)
        {
            return _context.IncomeDiscount.First(e => e.Id == id);
        }
    }
}
