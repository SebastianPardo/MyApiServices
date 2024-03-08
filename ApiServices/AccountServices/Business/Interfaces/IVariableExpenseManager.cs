using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business.Interfaces
{
    public interface IVariableExpenseManager
    {
        IEnumerable<VariableExpense> GetAllByUserId(Guid userId);
        VariableExpense GetById(Guid id);
        VariableExpense Add(VariableExpense variableExpense);
        bool Update(VariableExpense variableExpense);
        bool Delete(VariableExpense variableExpense);
    }
}
