using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class SavingsPocketManager : ISavingsPocketManager
    {
        DatabaseContext context;
        public SavingsPocketManager(DatabaseContext context)
        {
            this.context = context;
        }


        public SavingsPocket Add(SavingsPocket savingsPocket)
        {
            savingsPocket = context.SavingsPocket.Add(savingsPocket).Entity;
            context.SaveChanges();
            return savingsPocket;
        }

        public bool Delete(SavingsPocket savingsPocket)
        {
            context.SavingsPocket.Remove(savingsPocket);
            return context.SaveChanges() > 0;
        }

        public IEnumerable<SavingsPocket> GetAllByUserId(Guid userId)
        {
            return context.SavingsPocket.Where(e => e.Id == userId);
        }

        public SavingsPocket GetById(Guid id)
        {
            return context.SavingsPocket.FirstOrDefault(x => x.Id == id);
        }

        public bool Update(SavingsPocket savingsPocket)
        {
            context.SavingsPocket.Update(savingsPocket);
            context.SaveChanges();
            return context.SaveChanges() > 0;
        }

    }
}
