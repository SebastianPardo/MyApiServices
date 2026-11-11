using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class FinancialAccountTypeManager : IFinancialAccountTypeManager
    {
        private DatabaseContext _context;
        public FinancialAccountTypeManager(DatabaseContext context)
        {
            _context = context;
        }
        public IEnumerable<FinancialAccountType> GetAll() => _context.FinancialAccountType.ToList();

        public FinancialAccountType GetByCode(string code) => _context.FinancialAccountType.FirstOrDefault(x => x.Code == code);

        public FinancialAccountType GetById(Guid id) => _context.FinancialAccountType.FirstOrDefault(x => x.Id == id);
    }
}
