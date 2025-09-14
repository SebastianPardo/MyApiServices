namespace AuthServices.Controllers;

using AuthServices.Business;
using AuthServices.Business.Interfaces;
using AuthServices.Models.ApiModels;
using AuthServices.Models.ExternalModels;
using AutoMapper;
using Famnances.Core.Entities;
using Famnances.Core.Errors;
using Famnances.Core.Security.Services.Interfaces;
using Famnances.Core.Utils.Services.Interface;
using Famnances.DataCore.Entities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

[ApiController]
[Route("Api/[controller]")]
public class AuthController : ControllerBase
{
    IAccountService _accountService;
    IMapper Mapper;
    IPasswordService _passwordService;
    GoogleReCaptcha _googleReCaptcha;
    ITokenHandler _tokenHandler;

    public AuthController(IMapper mapper, IConfiguration configuration, IAccountService accountService, IPasswordService passwordService, ITokenHandler tokenHandler)
    {
        _accountService = accountService;
        _passwordService = passwordService;
        _tokenHandler = tokenHandler;
        _googleReCaptcha = new GoogleReCaptcha(configuration);

    }


    [HttpPost("Authenticate")]
    public async Task<IActionResult> Authenticate(AuthenticateRequest model)
    {
#if DEBUG
        var email = "js.pardo.j@gmail.com";
        var password = "Test@User1";
        var googleReCaptchaString = string.Empty;
        var IP = HttpContext.Connection.RemoteIpAddress.ToString();
#else
        var email = model.Param_1;
        var password = model.Param_2;
        var googleReCaptchaString = model.Param_3;
        var IP = HttpContext.Connection.RemoteIpAddress.ToString();
#endif
        var account = _accountService.getByUserNameOrEmail(email);
        var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (account == null || !_passwordService.Compare(password, account.Password))
            throw new AppException("Username or password is incorrect");
        else if (!account.IsActive)
        {
            throw new AppException("User Account is Deactivated Please Contact Admin");
        }
#if DEBUG
#else
        else if (!await _googleReCaptcha.Validate(googleReCaptchaString))
        {   
            throw new AppException("Recaptcha validation failed");
        }
#endif
        TokenContent tokenContent = new TokenContent
        {
            UserId = account.Id,
            Email = account.Email,
            User = account.UserName
        };

        AuthenticateResponse response = new AuthenticateResponse
        {
            AccountId = account.Id,
            UserName = account.UserName,
            Email = account.Email,
            Token = _tokenHandler.GetToken(tokenContent)
        };

        return Ok(response);
    }

    [HttpPost("ExternalAuthenticate")]
    public async Task<IActionResult> ExternalAuthenticate([FromBody] AuthenticateRequest model)
    {
        var provider = model.Param_1;
        var accessToken = model.Param_2;
        var idToken = model.Param_3;
        bool firstLogin = false;

        UserInfo? userInfo = null;


        if (provider.Equals(GoogleDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
        {
            userInfo = await ValidateGoogleToken(accessToken);
        }
        else if (provider.Equals(FacebookDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
        {
            userInfo = await ValidateFacebookToken(accessToken);
        }

        if (userInfo != null)
        {

            var account = _accountService.getByUserNameOrEmail(userInfo.Email);
            if (account == null)
            {
                account = new Account
                {
                    Email = userInfo.Email,
                    UserName = userInfo.Email,
                    IsActive = true,
                    Password = _passwordService.Generate(),
                    AccounTypeId = Guid.Parse("db727d49-2de5-4029-9556-055872cdea55"),
                    LinkedSocialMedias = new List<LinkedSocialMedia>
                    {
                        new LinkedSocialMedia {
                            SocialMediaId = Guid.Parse("8CA31668-B21C-4D41-A635-DDAD787EFA59"),
                            ToLogin = true,
                            UserName = userInfo.Id,
                        }
                    }
                };
                account = _accountService.Add(account);
                firstLogin = true;
            }

            firstLogin = account.User == null;


            TokenContent tokenContent = new TokenContent
            {
                UserId = account.Id,
                Email = account.Email,
                User = account.UserName
            };

            AuthenticateResponse response = new AuthenticateResponse
            {
                AccountId = account.Id,
                FirstName = userInfo.GivenName,
                LastName = userInfo.FamilyName,
                UserName = account.UserName,
                Email = account.Email,
                Token = _tokenHandler.GetToken(tokenContent),
                IsFirstLogin = firstLogin
            };

            return Ok(response);
        }
        return Unauthorized();
    }

    [HttpGet("GetAccount/{id}")]
    public async Task<IActionResult> GetAccount(Guid id)
    {
        return Ok(_accountService.GetById(id));
    }

    private async Task<UserInfo?> ValidateGoogleToken(string idToken)
    {
        try
        {

            GoogleCredential credentials = GoogleCredential.FromAccessToken(idToken);
            var oauthSerivce = new Oauth2Service(new BaseClientService.Initializer { HttpClientInitializer = credentials });
            var userGet = oauthSerivce.Userinfo.V2.Me.Get();
            var userinfo = userGet.Execute();

            //var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            return new UserInfo (userinfo);
        }
        catch
        {
            return null;
        }
    }

    private async Task<UserInfo?> ValidateFacebookToken(string accessToken)
    {
        using var client = new HttpClient();
        var payload = await client.GetFromJsonAsync<FacebookUser>($"https://graph.facebook.com/me?fields=id,email,first_name,middle_name,last_name,picture&access_token={accessToken}");
        if (payload == null || string.IsNullOrEmpty(payload.Email))
            return null;        
        return new UserInfo(payload);
    }


    //[AllowAnonymous]
    //[HttpPost("GoogleAuthenticate")]
    //public async Task<IActionResult> GoogleAuthenticate(AuthenticateRequest model)
    //{
    //    try
    //    {
    //        var email = model.Param_1;
    //        var token = model.Param_2;

    //        bool firstLogin = false;

    //        GoogleCredential cred2 = GoogleCredential.FromAccessToken(token);
    //        var oauthSerivce = new Oauth2Service(new BaseClientService.Initializer { HttpClientInitializer = cred2 });
    //        var userGet = oauthSerivce.Userinfo.V2.Me.Get();
    //        var userinfo = userGet.Execute();

    //        var account = _accountService.getByUserNameOrEmail(userinfo.Email);
    //        if (account == null)
    //        {
    //            account = new Account
    //            {
    //                Email = email,
    //                UserName = email,
    //                IsActive = true,
    //                Password = _passwordService.GeneratePassword(),
    //                AccounTypeId = Guid.Parse("db727d49-2de5-4029-9556-055872cdea55"),
    //                LinkedSocialMedias = new List<LinkedSocialMedia>
    //                {
    //                    new LinkedSocialMedia {
    //                        SocialMediaId = Guid.Parse("8CA31668-B21C-4D41-A635-DDAD787EFA59"),
    //                        ToLogin = true,
    //                        UserName = userinfo.Id,
    //                    }
    //                }
    //            };
    //            account = _accountService.Add(account);
    //            firstLogin = true;
    //        }

    //        firstLogin = account.User == null;

    //        TokenContent tokenContent = new TokenContent
    //        {
    //            UserId = account.Id,
    //            Email = account.Email,
    //            User = account.UserName
    //        };

    //        AuthenticateResponse response = new AuthenticateResponse
    //        {
    //            AccountId = account.Id,
    //            FirstName = userinfo.GivenName,
    //            LastName = userinfo.FamilyName,
    //            UserName = account.UserName,
    //            Email = account.Email,
    //            Token = _tokenHandler.GetToken(tokenContent),
    //            IsFirstLogin = firstLogin
    //        };

    //        return Ok(response);
    //    }
    //    catch
    //    {
    //        return BadRequest();
    //    }
    //}


    
}