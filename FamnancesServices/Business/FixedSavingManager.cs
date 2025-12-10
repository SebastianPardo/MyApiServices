using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Business
{
    public class FixedSavingManager : IFixedSavingManager
    {
        DatabaseContext _context;

        public FixedSavingManager(DatabaseContext context)
        {
            _context = context;
        }

        public FixedSaving Add(FixedSaving fixedSaving)
        {
            fixedSaving = _context.Add(fixedSaving).Entity;
            _context.SaveChanges();
            return fixedSaving;
        }

        public bool Delete(FixedSaving fixedSaving)
        {
            _context.Remove(fixedSaving);
            return _context.SaveChanges() > 0;
        }

        public IEnumerable<FixedSaving> GetAllByUserId(Guid userId)
        {
            return _context.FixedSaving.Include(e => e.SavingsPocket).Include(e=>e.SavingSource).Include(e => e.Periodicity).Where(e => e.SavingsPocket.UserId == userId);
        }

        public FixedSaving GetById(Guid userId, Guid id)
        {
            return _context.FixedSaving.Single(e => e.Id == id && e.SavingsPocket.UserId == userId);
        }

        public bool Update(FixedSaving fixedSaving)
        {
            _context.FixedSaving.Update(fixedSaving);
            return _context.SaveChanges() > 0;
        }
    }
}
