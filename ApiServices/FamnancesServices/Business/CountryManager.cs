using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class CountryManager : ICountryManager
    {
        DatabaseContext _context;
        public CountryManager(DatabaseContext context)
        {
            _context = context;
        }
        public List<Country> GetAll()
        {
            return _context.Country.ToList();
        }
    }
}
