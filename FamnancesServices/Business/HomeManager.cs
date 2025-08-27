using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class HomeManager : IHomeManager
    {
        DatabaseContext context;
        public HomeManager(DatabaseContext context)
        {
            this.context = context;
        }

        public bool Add(Home home)
        {
            context.Home.Add(home);
            return context.SaveChanges() > 0;
        }

        public bool Delete(Home home)
        {
            context.Home.Remove(home);
            return context.SaveChanges() > 0;
        }


        public Home? GetById(Guid id)
        {
            return context.Home.FirstOrDefault(u => u.Id == id);
        }

        public Home? GetByUser(Guid userId)
        {
            return context.Home.FirstOrDefault(e=>e.Users.Any( ee => ee.Id == userId));
        }

        public Home Update(Home home)
        {
            home = context.Home.Update(home).Entity;
            context.SaveChanges();
            return home;
        }
    }
}
