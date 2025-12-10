using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface ISavingSourceManager
    {
        List<SavingSource> GetAll();
        SavingSource GetByCode(string code);
    }
}