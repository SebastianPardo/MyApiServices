using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business.Interfaces
{
    public interface IUserManager
    {
        User? getByUserOrEmail(string user);
        List<User> GetAll();
        User GetById(int id);
        User Add(User user);
        User Update(User user);
        bool Delete(User user);

    }
}
