using AccountServices.Business.Interfaces;
using OhMyMoney.DataCore.Data;
using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business
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

        public User Update(User user)
        {
            user = context.User.Update(user).Entity;
            context.SaveChanges();
            return user;
        }

        IEnumerable<User> IUserManager.GetAll()
        {
            return context.User;
        }

        User IUserManager.GetById(Guid id)
        {
            return context.User.FirstOrDefault(x => x.Id == id);
        }

        User? IUserManager.getByUserNameOrEmail(string accountEmail)
        {
            return context.User.FirstOrDefault(x => x.Account.Email == accountEmail || x.Account.UserName == accountEmail);
        }
    }
}
