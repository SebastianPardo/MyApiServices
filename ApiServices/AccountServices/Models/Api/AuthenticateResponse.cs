namespace AccountServices.Models.Api;

public class AuthenticateResponse
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }    
    public string Email { get; set; }
    public string Token { get; set; }
}