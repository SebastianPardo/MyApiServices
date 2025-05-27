using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IPeriodManager
    {
        IEnumerable<Period> GetAll();
    }
}