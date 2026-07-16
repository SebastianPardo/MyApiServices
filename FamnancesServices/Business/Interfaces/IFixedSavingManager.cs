using Famnances.DataCore.Entities;
using FamnancesServices.Models;

namespace FamnancesServices.Business.Interfaces
{
    public interface IFixedSavingManager
    {
        IEnumerable<FixedSaving> GetAllByUserId(Guid userId);
        FixedSaving GetById(Guid userId, Guid id);
        FixedSaving Add(FixedSaving fixedSaving);
        bool Update(FixedSaving fixedSaving);
        bool Delete(FixedSaving fixedSaving);
        List<SummaryFixedSavingsModel> Summary(Guid userId, Guid? homeId, DateTime dateTime);
    }
}
