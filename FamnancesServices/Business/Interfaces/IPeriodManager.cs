using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IPeriodManager
    {
        IEnumerable<Period> GetAll();
        Period GetByCode(string code);
        Period GetById(Guid id);
    }
}