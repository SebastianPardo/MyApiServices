
using AccountServices.Business.Interfaces;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;

namespace AccountServices.Business
{
    public class OutflowManager : IOutflowManager
    {
        DatabaseContext context;
        public OutflowManager(DatabaseContext context)
        {
            this.context = context;
        }

        public Outflow Add(Outflow variableExpense)
        {
            variableExpense = context.VariableExpense.Add(variableExpense).Entity;
            context.SaveChanges();
            return variableExpense;
        }

        public bool Delete(Outflow variableExpense)
        {
            context.VariableExpense.Remove(variableExpense);
            return context.SaveChanges() > 0;
        }

        public bool Update(Outflow variableExpense)
        {
            context.VariableExpense.Update(variableExpense);
            context.SaveChanges();
            return context.SaveChanges() > 0;
        }

        IEnumerable<Outflow> GetAllByUserId(Guid userId)
        {
            return context.VariableExpense.Where(fe => fe.ExpensesBudget.UserId == userId);
        }

        IEnumerable<Outflow> IOutflowManager.GetAllByUserId(Guid userId)
        {
            throw new NotImplementedException();
        }

        Outflow GetById(Guid id)
        {
            return context.VariableExpense.FirstOrDefault(x => x.Id == id);
        }

        Outflow IOutflowManager.GetById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
