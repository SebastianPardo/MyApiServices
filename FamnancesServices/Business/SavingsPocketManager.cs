using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            return context.SavingsPocket.Include(e=>e.SavingsRecords).Where(e => e.UserId  == userId);
        }

        public IEnumerable<SavingsPocket> GetAllByHome(Guid homeId)
        {
            return context.SavingsPocket.Include(e => e.SavingsRecords).Where(e => e.User.HomeId == homeId);
        }

        public SavingsPocket GetById(Guid id)
        {
            return context.SavingsPocket.Include(e=>e.SavingsRecords).FirstOrDefault(x => x.Id == id);
        }

        public bool Update(SavingsPocket savingsPocket)
        {
            context.SavingsPocket.Update(savingsPocket);
            return context.SaveChanges() > 0;
        }

    }
}
