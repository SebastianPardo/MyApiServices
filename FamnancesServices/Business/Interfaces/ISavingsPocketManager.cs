using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface ISavingsPocketManager
    {
        IEnumerable<SavingsPocket> GetAll(Guid userId);
        SavingsPocket GetById(Guid id);
        SavingsPocket Add(SavingsPocket savingsPocket);
        bool Update(SavingsPocket savingsPocket);
        bool Delete(SavingsPocket savingsPocket);
    }
}
