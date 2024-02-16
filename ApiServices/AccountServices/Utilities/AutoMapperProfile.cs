namespace AccountServices.Utilities;
using AutoMapper;
using OhMyMoney.DataCore.Entities;
using AccountServices.Models.Api;
using OhMyMoney.AuthMiddleware.Models;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User -> AuthenticateResponse
        CreateMap<User, AuthenticateResponse>()
            .ForMember(d => d.UserId, o => o.MapFrom(p => p.UserId));

        //AuthenticateRequest -> AuthenticationRequest
        CreateMap<AuthenticateRequest, AuthenticationRequest>()
            .ForMember(d => d.EmailUser, o => o.MapFrom(p => p.Param_1))
            .ForMember(d => d.Password, o => o.MapFrom(p => p.Param_2)); ; 
        ;

        // RegisterRequest -> User
        CreateMap<RegisterRequest, User>();

        // UpdateRequest -> User
        CreateMap<UpdateRequest, User>()
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