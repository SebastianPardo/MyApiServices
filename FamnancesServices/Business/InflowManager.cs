using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class InflowManager: IInflowManager
    {
        DatabaseContext context;
        public InflowManager(DatabaseContext context)
        {
            this.context = context;
        }

        public Inflow Add(Inflow inflow)
        {
            inflow = context.Inflow.Add(inflow).Entity;
            context.SaveChanges();
            return inflow;
        }

        public bool Delete(Inflow inflow)
        {
            context.Inflow.Remove(inflow);
            return context.SaveChanges() > 0;
        }

        public bool Update(Inflow inflow)
        {
            context.Inflow.Update(inflow);
            context.SaveChanges();
            return context.SaveChanges() > 0;
        }
    }
}
