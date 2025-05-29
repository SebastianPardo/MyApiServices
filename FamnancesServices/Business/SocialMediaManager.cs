using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class SocialMediaManager : ISocialMediaManager
    {
        DatabaseContext context;
        public SocialMediaManager(DatabaseContext context)
        {
            this.context = context;
        }

        public bool Add(SocialMedia entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(SocialMedia entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SocialMedia> GetAll()
        {
            throw new NotImplementedException();
        }

        public SocialMedia GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public SocialMedia Update(SocialMedia entity)
        {
            throw new NotImplementedException();
        }
    }
}
