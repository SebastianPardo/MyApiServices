using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IOutflowManager
    {
        IEnumerable<Outflow> GetAllByUserId(Guid userId);
        Outflow GetById(Guid id);
        Outflow Add(Outflow variableExpense);
        bool Update(Outflow variableExpense);
        bool Delete(Outflow variableExpense);
        decimal GetByPeriod(DateTime startDate, DateTime endDate, Guid userId);
    }
}
