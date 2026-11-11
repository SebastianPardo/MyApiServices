using Famnances.DataCore.Data;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class FinancialInstitutionManager : IFinancialInstitutionManager
    {
        private DatabaseContext _context;
        public FinancialInstitutionManager(DatabaseContext context)
        {
            _context = context;
        }
        public object GetAll() => _context.FinancialInstitution.ToList();

        public object GetByCode(string code) => _context.FinancialInstitution.FirstOrDefault(x => x.Code == code);

        public object GetById(Guid id) => _context.FinancialInstitution.FirstOrDefault(x => x.Id == id);
    }
}
