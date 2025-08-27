using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface IUserManager
    {
        User? getByUserNameOrEmail(string accountEmail);
        IEnumerable<User> GetAll();
        User GetById(Guid id);
        List<User> Search (string email, bool admisitrator);
        bool Add(User user);
        User Update(User user);
        bool Delete(User user);

    }
}
