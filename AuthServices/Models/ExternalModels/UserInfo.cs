using Google.Apis.Oauth2.v2.Data;

namespace AuthServices.Models.ExternalModels
{
    public class UserInfo
    {

        public UserInfo(Userinfo userinfo)
        {
            Id = userinfo.Id;
            Email = userinfo.Email;
            GivenName = userinfo.GivenName;
            FamilyName = userinfo.FamilyName;
            Picture = userinfo.Picture;
        }

        public UserInfo(FacebookUser payload)
        {
            Id = payload.Id;
            Email = payload.Email;
            GivenName = $"{payload.FirstName} {payload.MiddleName}" ;
            FamilyName = payload.LastName;
            Picture = payload.Picture;
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Picture { get; set; }
    }
}
