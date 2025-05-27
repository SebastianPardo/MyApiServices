using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class CityManager : ICityManager
    {
        DatabaseContext _context;
        public CityManager(DatabaseContext context)
        {
            _context = context;
        }

        public List<City> GetAllByProvince(Guid provinceId)
        {
            return _context.City.Where(e => e.ProvinceId == provinceId).ToList();
        }
    }
}
