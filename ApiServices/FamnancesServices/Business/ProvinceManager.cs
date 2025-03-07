using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class ProvinceManager : IProvinceManager
    {
        DatabaseContext _context;
        public ProvinceManager(DatabaseContext context)
        {
            _context = context;
        }

        public List<Province> GetAllByCountry(Guid countryId)
        {
            return _context.Province.Where(e => e.CountryId == countryId).ToList();
        }
    }
}
