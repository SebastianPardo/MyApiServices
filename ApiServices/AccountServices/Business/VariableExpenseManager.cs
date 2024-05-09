
using AccountServices.Business.Interfaces;
using OhMyMoney.DataCore.Data;
using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business
{
    public class VariableExpenseManager : IVariableExpenseManager
    {
        DatabaseContext context;
        public VariableExpenseManager(DatabaseContext context)
        {
            this.context = context;
        }

        public VariableExpense Add(VariableExpense variableExpense)
        {
            variableExpense = context.VariableExpense.Add(variableExpense).Entity;
            context.SaveChanges();
            return variableExpense;
        }

        public bool Delete(VariableExpense variableExpense)
        {
            context.VariableExpense.Remove(variableExpense);
            return context.SaveChanges() > 0;
        }

        public bool Update(VariableExpense variableExpense)
        {
            context.VariableExpense.Update(variableExpense);
            context.SaveChanges();
            return context.SaveChanges() > 0;
        }

        IEnumerable<VariableExpense> IVariableExpenseManager.GetAllByUserId(Guid userId)
        {
            return context.VariableExpense.Where(fe => fe.ExpensesBudget.UserId == userId);
        }

        VariableExpense GetById(Guid id)
        {
            return context.VariableExpense.FirstOrDefault(x => x.Id == id);
        }

        VariableExpense IVariableExpenseManager.GetById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
