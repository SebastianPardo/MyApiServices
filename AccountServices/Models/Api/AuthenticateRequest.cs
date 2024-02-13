namespace AccountServices.Models.Api;

using System.ComponentModel.DataAnnotations;

public class AuthenticateRequest
{
    [Required]
    public string Param_1 { get; set; }

    [Required]
    public string Param_2 { get; set; }

    public string? Param_3 { get; set; }
}