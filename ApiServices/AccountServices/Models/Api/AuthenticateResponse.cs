namespace AccountServices.Models.Api;

public class AuthenticateResponse
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }    
    public string Email { get; set; }
    public string Token { get; set; }
    public DateTime? LastLogin { get; set; }
}