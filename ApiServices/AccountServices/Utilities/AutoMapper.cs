namespace AccountServices.Utilities;
using global::AutoMapper;
using Famnances.DataCore.Entities;
using AccountServices.Models.Api;
using Famnances.AuthMiddleware.Models;

public class AutoMapper : Profile
{
    public AutoMapper()
    {
        // UpdateRequest -> User
        CreateMap<UpdateRequest, Account>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    // ignore null & empty string properties
                    if (prop == null) return false;
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                    return true;
                }
            ));
    }
}