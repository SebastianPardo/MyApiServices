using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;

namespace FamnancesServices.Business
{
    public class AccountTypeManager : IAccountTypeManager
    {
        DatabaseContext context;
        public AccountTypeManager(DatabaseContext context)
        {
            this.context = context;
        }

        public bool Add(AccountType entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(AccountType entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AccountType> GetAll()
        {
            throw new NotImplementedException();
        }

        public AccountType GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public AccountType Update(AccountType entity)
        {
            throw new NotImplementedException();
        }
    }
}
