using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class BudgetTypeManager : IBudgetTypeManager
    {
        DatabaseContext _context;
        public BudgetTypeManager(DatabaseContext context)
        {
            _context = context;
        }

        public ExpensesBudgetType GetByCode(string code)
        {
            return _context.ExpensesBudgetType.First(x => x.Code == code);
        }
    }
}
