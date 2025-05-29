using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface ILinkedSocialMediaManager
    {
        IEnumerable<LinkedSocialMedia> GetAll();
        LinkedSocialMedia GetById(Guid id);
        bool Add(LinkedSocialMedia entity);
        LinkedSocialMedia Update(LinkedSocialMedia entity);
        bool Delete(LinkedSocialMedia entity);

    }
}
