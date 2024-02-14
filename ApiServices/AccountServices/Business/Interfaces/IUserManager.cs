using OhMyMoney.DataCore.Entities;

namespace AccountServices.Business.Interfaces
{
    public interface IUserManager
    {
        User? getByUserOrEmail(string user);
        IEnumerable<User> GetAll();
        User GetById(Guid id);
        bool Add(User user);
        User Update(User user);
        bool Delete(User user);

    }
}
