using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface ICityManager
    {
        List<City> GetAllByProvince(Guid provinceId);
        City? GetByCode(string code);
        City? GetById(Guid id);
    }
}
