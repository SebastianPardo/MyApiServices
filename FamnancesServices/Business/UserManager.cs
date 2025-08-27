using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Business
{
    public class UserManager : IUserManager
    {
        DatabaseContext context;
        public UserManager(DatabaseContext context)
        {
            this.context = context;
        }

        public bool Add(User user)
        {
            context.User.Add(user);
            return context.SaveChanges() > 0;
        }

        public bool Delete(User user)
        {
            context.User.Remove(user);
            return context.SaveChanges() > 0;
        }

        public List<User> Search(string email, bool admisitrator)
        {
            if (admisitrator)
            {
                return context.User.Include(e => e.Account)
                    .Where(e => (e.LegalName.Contains(email) || e.Account.Email.Contains(email)) && e.HomeId == null).ToList();
            }
            else
            {
                return context.User.Include(e => e.Account)
                    .Where(e => (e.LegalName.Contains(email) || e.Account.Email.Contains(email)) && e.HomeId != null && e.HomeAdministrator == true).ToList();
            }

        }

        public User Update(User user)
        {
            user = context.User.Update(user).Entity;
            context.SaveChanges();
            return user;
        }

        IEnumerable<User> GetAll()
        {
            return context.User;
        }

        IEnumerable<User> IUserManager.GetAll()
        {
            return GetAll();
        }

        User GetById(Guid id)
        {
            return context.User.FirstOrDefault(x => x.Id == id);
        }

        User IUserManager.GetById(Guid id)
        {
            return GetById(id);
        }

        User? getByUserNameOrEmail(string accountEmail)
        {
            return context.User.FirstOrDefault(x => x.Account.Email == accountEmail || x.Account.UserName == accountEmail);
        }

        User? IUserManager.getByUserNameOrEmail(string accountEmail)
        {
            return getByUserNameOrEmail(accountEmail);
        }
    }
}
