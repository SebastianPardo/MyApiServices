using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class OutflowManager : IOutflowManager
    {
        DatabaseContext context;
        public OutflowManager(DatabaseContext context)
        {
            this.context = context;
        }

        public Outflow Add(Outflow outflow)
        {
            outflow = context.Outflow.Add(outflow).Entity;
            context.SaveChanges();
            return outflow;
        }

        public bool Delete(Outflow outflow)
        {
            context.Outflow.Remove(outflow);
            return context.SaveChanges() > 0;
        }

        public bool Update(Outflow outflow)
        {
            context.Outflow.Update(outflow);
            context.SaveChanges();
            return context.SaveChanges() > 0;
        }

        public IEnumerable<Outflow> GetAllByUserId(Guid userId)
        {
            return context.Outflow.Where(fe => fe.ExpensesBudget.UserId == userId);
        }

        public Outflow GetById(Guid id)
        {
            return context.Outflow.FirstOrDefault(x => x.Id == id);
        }
    }
}
