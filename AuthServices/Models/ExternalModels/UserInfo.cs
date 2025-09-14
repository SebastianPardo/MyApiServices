using Google.Apis.Auth;
using Google.Apis.Oauth2.v2.Data;

namespace AuthServices.Models.ExternalModels
{
    public class UserInfo
    {
        public UserInfo(GoogleJsonWebSignature.Payload payload)
        {
            Id = payload.Subject;
            Email = payload.Email;
            GivenName = payload.GivenName;
            FamilyName = payload.FamilyName;
            Picture = payload.Picture;
        }
        public UserInfo(FacebookUser payload)
        {
            Id = payload.Id;
            Email = payload.Email;
            GivenName = $"{payload.FirstName} {payload.MiddleName}" ;
            FamilyName = payload.LastName;
            Picture = payload.Picture;
        }

        public UserInfo(Userinfo payload)
        {
            Id = payload.Id;
            Email = payload.Email;
            GivenName = payload.GivenName;
            FamilyName = payload.FamilyName;
            Picture = payload.Picture;
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Picture { get; set; }
    }
}
