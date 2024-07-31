using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IUserManager
    {
        User? getByUserNameOrEmail(string accountEmail);
        IEnumerable<User> GetAll();
        User GetById(Guid id);
        bool Add(User user);
        User Update(User user);
        bool Delete(User user);

    }
}
