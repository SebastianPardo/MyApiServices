using Famnances.DataCore.Entities;

namespace FamnancesServices.Business.Interfaces
{
    public interface ISocialMediaManager
    {
        IEnumerable<SocialMedia> GetAll();
        SocialMedia GetById(Guid id);
        bool Add(SocialMedia entity);
        SocialMedia Update(SocialMedia entity);
        bool Delete(SocialMedia entity);

    }
}
