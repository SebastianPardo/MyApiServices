namespace AccountServices.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Famnances.AuthMiddleware.Entities;
using Microsoft.Extensions.Options;
using Famnances.AuthMiddleware;
using AuthorizeAttribute = Famnances.AuthMiddleware.AuthorizeAttribute;
using AccountServices.Models.ApiModels;
using AccountServices.Business;
using AccountServices.Business.Interfaces;
using Famnances.DataCore.Entities;

[Authorize]
[ApiController]
[Route("Api/[controller]")]
public class AccountController : ControllerBase
{
    IAccountManager _accountManager;
    IMapper Mapper;
    readonly GoogleReCaptcha GoogleReCaptcha;
    readonly JwtTokenHandler JwtTokenHandler;

    public AccountController(IMapper mapper, IConfiguration configuration, IAccountManager accountManager, IOptions<AppSettings> appSettings)
    {
        _accountManager = accountManager;
        Mapper = mapper;
        GoogleReCaptcha = new GoogleReCaptcha(configuration);
        JwtTokenHandler = new JwtTokenHandler(appSettings.Value);
    }

    [AllowAnonymous]
    [HttpPost("Authenticate")]
    public async Task<IActionResult> Authenticate(AuthenticateRequest model)
    {
#if DEBUG
        var email = "WebDev+1@huntmarketingservices.com";
        var password = "Test@User1";
        var googleReCaptchaString = string.Empty;
        var IP = HttpContext.Connection.RemoteIpAddress.ToString();
#else
        var email = model.Param_1;
        var password = model.Param_2;
        var googleReCaptchaString = model.Param_3;
        var IP = HttpContext.Connection.RemoteIpAddress.ToString();
#endif
        var user = _accountManager.getByUserNameOrEmail(email);
        var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (user == null || !Utilities.ComparePassword(password, user.Password))
            throw new AppException("Username or password is incorrect");
        else if (!user.IsActive)
        {
            throw new AppException("User Account is Deactivated Please Contact Admin");
        }
        //No validate recaptcha when is testing that's why ignore local host and Office Ip.
        else if (!await GoogleReCaptcha.Validate(googleReCaptchaString) && enviroment != "Development" && IP != "::1" && IP != "70.52.127.163")
        {
            throw new AppException("Recaptcha validation failed");
        }
        TokenContent tokenContent = new TokenContent
        {
            UserId = user.Id,
            Email = user.Email,
            User = user.UserName
        };

        AuthenticateResponse response = new AuthenticateResponse
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Token = JwtTokenHandler.GenerateToken(tokenContent)
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

            GoogleCredential cred2 = GoogleCredential.FromAccessToken(token);
            var oauthSerivce = new Oauth2Service(new BaseClientService.Initializer { HttpClientInitializer = cred2 });
            var userGet = oauthSerivce.Userinfo.V2.Me.Get();
            var userinfo = userGet.Execute();

            var user = _accountManager.getByUserNameOrEmail(userinfo.Email);
            if (user.Email == email)
            {
                TokenContent tokenContent = new TokenContent
                {
                    UserId = user.Id,
                    Email = user.Email,
                    User = user.UserName
                };

                AuthenticateResponse response = new AuthenticateResponse
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = JwtTokenHandler.GenerateToken(tokenContent)
                };

                return Ok(response);
            }
            return BadRequest();
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var user = _accountManager.GetById(id);
        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest model)
    {
        var user = _accountManager.getByUserNameOrEmail(model.Username);
        if (user != null)
            throw new AppException("Username '" + model.Username + "' is already taken");
        user = Mapper.Map<Account>(model);
        user.Password = Utilities.ValidatePassword(model.Password);
        if (user.Password == null)
            throw new AppException("Minimum of different classes of characters in password is 3. Classes of characters: Lower Case, Upper Case, Digits, Special Characters.");
        _accountManager.Add(user);
        return Ok(new { message = "Registration successful" });
    }

    [HttpPut("{id}")]
    public IActionResult Update(UpdateRequest model)
    {
        var user = _accountManager.getByUserNameOrEmail(model.Username);
        if (user == null || !Utilities.ComparePassword(model.Password, user.Password))
            throw new AppException("Username or password is incorrect");

        Mapper.Map(model, user);
        user.Password = Utilities.ValidatePassword(model.Password);
        if (user.Password == null)
            throw new AppException("Minimum of different classes of characters in password is 3. Classes of characters: Lower Case, Upper Case, Digits, Special Characters.");

        _accountManager.Update(user);
        return Ok(new { message = "User updated successfully" });
    }
}