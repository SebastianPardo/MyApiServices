using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IProvinceManager
    {
        List<Province> GetAllByCountry(Guid countryId);
    }
}
