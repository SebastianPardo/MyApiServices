using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class ErrorLogManager : IErrorLogManager
    {
        DatabaseContext _context;
        public ErrorLogManager(DatabaseContext context)
        {
            _context = context;
        }

        public ErrorLog? Add(ErrorLog error)
        {
            error = _context.ErrorLog.Add(error).Entity;
            _context.SaveChanges();
            return error;

        }

        public ErrorLog? GetById(Guid id)
        {
            return _context.ErrorLog.SingleOrDefault(e => e.Id == id);
        }
    }
}
