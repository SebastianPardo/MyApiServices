using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class SavingSourceManager : ISavingSourceManager
    {
        DatabaseContext _context;
        public SavingSourceManager(DatabaseContext context)
        {
            _context = context;
        }
        
        public List<SavingSource> GetAll()
        {
            return _context.SavingSource.ToList();
        }

        public SavingSource GetByCode(string code)
        {
            return _context.SavingSource.First(x => x.Code == code);
        }
    }
}
