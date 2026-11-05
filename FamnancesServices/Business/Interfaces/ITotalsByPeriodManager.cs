using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface ITotalsByPeriodManager
    {
        bool Exist(Guid userId, DateTime date);
        PeriodDates? GetMostRecent(Guid userId);
        PeriodDates? GetByCurrentDay(Guid id);
        PeriodDates? GetByDate(Guid userId, DateTime date);
        PeriodDates Save(PeriodDates PeriodDates);
    }
}