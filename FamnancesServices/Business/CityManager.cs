using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public City? GetByCode(string code)
        {
            return _context.City.Include(e => e.Province).ThenInclude(e => e.Country).Single(e => e.Code == code);
        }

        public City? GetById(Guid id)
        {
            return _context.City.Include(e => e.Province).ThenInclude(e => e.Country).Single(e => e.Id == id);
        }
    }
}
