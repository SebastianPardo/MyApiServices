using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface ITotalsByPeriodManager
    {
        TotalsByPeriod? GetByCurrentDay(Guid id);
        TotalsByPeriod? GetByCurrentPeriod(Guid userId);
        TotalsByPeriod Save(TotalsByPeriod totalsByPeriod);
    }
}