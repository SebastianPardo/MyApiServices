namespace AccountServices.Business.Interfaces
{
    public interface IUtilityService
    {
        bool ComparePassword(string password, byte[] hashPass);
        byte[] GeneratePassword();
        byte[] ValidatePassword(string password);
    }
}