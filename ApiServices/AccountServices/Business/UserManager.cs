using AccountServices.Business.Interfaces;
using OhMyMoney.DataCore.Data;
using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business
{
    public class UserManager : IUserManager
    {
        public UserManager(DatabaseContext databaseContext) { }

        public User Add(User user)
        {
            throw new NotImplementedException();
        }

        public bool Delete(User user)
        {
            throw new NotImplementedException();
        }

        public List<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetById(int id)
        {
            throw new NotImplementedException();
        }

        public User? getByUserOrEmail(string user)
        {
            throw new NotImplementedException();
        }

        public User Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
