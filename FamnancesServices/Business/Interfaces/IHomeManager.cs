using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IHomeManager
    {
        Home? GetByUser(Guid userId);
        Home GetById(Guid id);
        Home GetComplete(Guid userId, DateTime date);
        bool Add(Home home);
        Home Update(Home home);
        bool Delete(Home home);
    }
}
