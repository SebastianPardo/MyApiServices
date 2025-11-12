using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IErrorLogManager
    {
        ErrorLog? Add(ErrorLog error);
        ErrorLog? GetById(Guid id);
    }
}
