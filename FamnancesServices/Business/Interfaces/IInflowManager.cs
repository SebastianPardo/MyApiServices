using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IInflowManager
    {
        Inflow Add(Inflow entity);
        bool Delete(Inflow inflow);
        bool Update(Inflow inflow);
    }
}
