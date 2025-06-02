namespace AccountServices.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Famnances.AuthMiddleware.Entities;
using Famnances.AuthMiddleware;
using AuthorizeAttribute = Famnances.AuthMiddleware.AuthorizeAttribute;
using AccountServices.Models.ApiModels;
using AccountServices.Business;
using AccountServices.Business.Interfaces;
using Famnances.DataCore.Entities;
using Famnances.AuthMiddleware.Interfaces;

[Authorize]
[ApiController]
[Route("Api/[controller]")]
public class AuthController : ControllerBase
{
    IAccountService _accountService;
    IMapper Mapper;
    IUtilityService _utilityService;
    GoogleReCaptcha _googleReCaptcha;
    ITokenHandler _tokenHandler;

    public AuthController(IMapper mapper, IConfiguration configuration, IAccountService accountService, IUtilityService utilityService, ITokenHandler tokenHandler)
    {
        _accountService = accountService;
        _utilityService = utilityService;
        _tokenHandler = tokenHandler;
        Mapper = mapper;
        _googleReCaptcha = new GoogleReCaptcha(configuration);

    }

    [AllowAnonymous]
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
        if (account == null || !_utilityService.ComparePassword(password, account.Password))
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

    [AllowAnonymous]
    [HttpPost("GoogleAuthenticate")]
    public async Task<IActionResult> GoogleAuthenticate(AuthenticateRequest model)
    {
        try
        {
            var email = model.Param_1;
            var token = model.Param_2;

            bool firstLogin = false;

            GoogleCredential cred2 = GoogleCredential.FromAccessToken(token);
            var oauthSerivce = new Oauth2Service(new BaseClientService.Initializer { HttpClientInitializer = cred2 });
            var userGet = oauthSerivce.Userinfo.V2.Me.Get();
            var userinfo = userGet.Execute();

            var account = _accountService.getByUserNameOrEmail(userinfo.Email);
            if (account == null)
            {
                account = new Account
                {
                    Email = email,
                    UserName = email,
                    IsActive = true,
                    Password = _utilityService.GeneratePassword(),
                    AccounTypeId = Guid.Parse("db727d49-2de5-4029-9556-055872cdea55"),
                    LinkedSocialMedias = new List<LinkedSocialMedia>
                    {
                        new LinkedSocialMedia {
                            SocialMediaId = Guid.Parse("8CA31668-B21C-4D41-A635-DDAD787EFA59"),
                            ToLogin = true,
                            UserName = userinfo.Id,
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
                FirstName = userinfo.GivenName,
                LastName = userinfo.FamilyName,
                UserName = account.UserName,
                Email = account.Email,
                Token = _tokenHandler.GetToken(tokenContent),
                IsFirstLogin = firstLogin
            };

            return Ok(response);
        }
        catch
        {
            return BadRequest();
        }
    }
}