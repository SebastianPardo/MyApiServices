using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface ICityManager
    {
        List<City> GetAllByProvince(Guid provinceId);
        City? GetById(Guid id);
    }
}
