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

        public IEnumerable<User> GetAll()
        {
            return context.User;
        }

        public User GetById(Guid id)
        {
            return context.User.FirstOrDefault(x => x.UserId == id);
        }

        public User? getByUserOrEmail(string user)
        {
            return context.User.FirstOrDefault(x => x.Email == user || x.UserName == user);
        }

        public User Update(User user)
        {
            user = context.User.Update(user).Entity;
            context.SaveChanges();
            return user;
        }
    }
}
