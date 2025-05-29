using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class LinkedSocialMediaManager : ILinkedSocialMediaManager

    {
        DatabaseContext context;
        public LinkedSocialMediaManager(DatabaseContext context)
        {
            this.context = context;
        }

        public bool Add(LinkedSocialMedia entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(LinkedSocialMedia entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LinkedSocialMedia> GetAll()
        {
            throw new NotImplementedException();
        }

        public LinkedSocialMedia GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public LinkedSocialMedia Update(LinkedSocialMedia entity)
        {
            throw new NotImplementedException();
        }
    }
}
