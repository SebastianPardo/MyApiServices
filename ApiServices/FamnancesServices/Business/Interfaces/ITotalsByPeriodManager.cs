using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface ITotalsByPeriodManager
    {
        TotalsByPeriod? GetByCurrentPeriod();
    }
}