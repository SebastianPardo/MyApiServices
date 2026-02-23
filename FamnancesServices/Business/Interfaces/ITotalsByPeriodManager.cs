using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface ITotalsByPeriodManager
    {
        TotalsByPeriod? GetMostRecent(Guid userId);
        TotalsByPeriod? GetByCurrentDay(Guid id);
        TotalsByPeriod? GetByDate(Guid userId, DateTime date);
        TotalsByPeriod Save(TotalsByPeriod totalsByPeriod);
    }
}