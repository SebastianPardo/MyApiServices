using Athentication.DataCore.Models;

namespace Athentication.DataCore.ApiModels;

public class AuthResponse
{
    public Guid AccountId { get; set; }
    
    public UserInfo UserInfo { get; set; }
    public string Token { get; set; }
    public string Language { get; set; }
    public bool IsFirstLogin { get; set; }
}