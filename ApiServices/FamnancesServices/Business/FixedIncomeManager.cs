using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class FixedIncomeManager : IFixedIncomeManager
    {
        DatabaseContext context;
        public FixedIncomeManager(DatabaseContext context)
        {
            this.context = context;
        }

        public FixedIncome Add(FixedIncome fixedIncome)
        {
            fixedIncome = context.FixedIncome.Add(fixedIncome).Entity;
            context.SaveChanges();
            return fixedIncome;
        }

        public bool Delete(FixedIncome fixedIncome)
        {
            context.FixedIncome.Remove(fixedIncome);
            return context.SaveChanges() > 0;
        }

        public IEnumerable<FixedIncome> GetAllByUserId(Guid userId)
        {
            return context.FixedIncome.Where(fe => fe.UserId == userId);
        }

        public FixedIncome GetById(Guid id)
        {
            return context.FixedIncome.FirstOrDefault(x => x.Id == id);
        }

        public bool Update(FixedIncome fixedIncome)
        {
            context.FixedIncome.Update(fixedIncome);
            context.SaveChanges();
            return context.SaveChanges() > 0;
        }
    }
}
