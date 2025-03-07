using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface ICountryManager
    {
        List<Country> GetAll();
    }
}
