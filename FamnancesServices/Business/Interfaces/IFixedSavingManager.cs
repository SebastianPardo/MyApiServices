using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IFixedSavingManager
    {
        IEnumerable<FixedSaving> GetAllByUserId(Guid userId);
        FixedSaving GetById(Guid userId, Guid id);
        FixedSaving Add(FixedSaving fixedSaving);
        bool Update(FixedSaving fixedSaving);
        bool Delete(FixedSaving fixedSaving);
    }
}
