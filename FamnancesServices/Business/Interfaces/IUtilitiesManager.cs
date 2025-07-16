using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IUtilitiesManager
    {
        (DateTime, DateTime) GetPeriodDates(Guid periodId, int dayStart);
    }
}
