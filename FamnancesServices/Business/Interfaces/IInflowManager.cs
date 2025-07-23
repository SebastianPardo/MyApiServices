using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IInflowManager
    {
        Inflow GetById(Guid id);
        List<Inflow> GetAllByPeriod(DateTime startDate, DateTime endDate, Guid userId);
        Inflow Add(Inflow entity);
        bool Delete(Inflow inflow);
        bool Update(Inflow inflow);
        decimal GetTotalByPeriod(DateTime startDate, DateTime endDate, Guid userId);
    }
}
