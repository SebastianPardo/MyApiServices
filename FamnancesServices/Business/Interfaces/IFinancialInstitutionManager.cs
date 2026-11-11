namespace FamnancesServices.Business.Interfaces
{
    public interface IFinancialInstitutionManager
    {
        object GetAll();
        object GetByCode(string code);
        object GetById(Guid id);
    }
}