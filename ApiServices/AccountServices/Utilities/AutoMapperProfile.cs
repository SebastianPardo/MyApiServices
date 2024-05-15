namespace AccountServices.Utilities;
using AutoMapper;
using Famnances.DataCore.Entities;
using AccountServices.Models.Api;
using Famnances.AuthMiddleware.Models;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Account -> AuthenticateResponse
        CreateMap<Account, AuthenticateResponse>()
            .ForMember(d => d.UserId, o => o.MapFrom(p => p.Id));

        //AuthenticateRequest -> AuthenticationRequest
        CreateMap<AuthenticateRequest, AuthenticationRequest>()
            .ForMember(d => d.EmailUser, o => o.MapFrom(p => p.Param_1))
            .ForMember(d => d.Password, o => o.MapFrom(p => p.Param_2)); ; 
        ;

        // RegisterRequest -> Account
        CreateMap<RegisterRequest, Account>();

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