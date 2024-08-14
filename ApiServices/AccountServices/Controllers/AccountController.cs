namespace AccountServices.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AccountServices.Utilities;
using AccountServices.Models.Api;
using Famnances.DataCore.Entities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using AccountServices.Business.Interfaces;
using Famnances.AuthMiddleware;
using Famnances.AuthMiddleware.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using AuthorizeAttribute = Authorization.AuthorizeAttribute;

[Authorize]
[ApiController]
[Route("Api/[controller]")]
public class AccountController : ControllerBase
{
    IAccountManager accountManager;
    IMapper Mapper;
    readonly JwtTokenHandler JwtTokenHandler;

    public AccountController(IMapper mapper, IAccountManager accountManager, IOptions<AppSettings> appSettings)
    {
        this.accountManager = accountManager;
        Mapper = mapper;
        JwtTokenHandler = new JwtTokenHandler(appSettings.Value);
    }

    [AllowAnonymous]
    [HttpPost("Authenticate")]
    public async Task<IActionResult> Authenticate(AuthenticateRequest model)
    {
        var email = model.Param_1;
        var password = model.Param_2;

        var account = accountManager.getByUserNameOrEmail(email);

        if (account == null || !GeneralUtilities.ComparePassword(password, account.Password))
            throw new AppException("Username or password is incorrect");
        else if (!account.IsActive)
        {
            throw new AppException("User Account is Deactivated Please Contact Admin");
        }
        
        TokenContent tokenContent = new TokenContent
        {
            UserId = account.Id,
            Email = account.Email,
            User = account.UserName
        };

        AuthenticateResponse response = new AuthenticateResponse
        {
            UserId = account.Id,
            UserName = account.UserName,
            Email = account.Email,
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

            var account = accountManager.getByUserNameOrEmail(userinfo.Email);
            if (account.Email == email)
            {
                TokenContent tokenContent = new TokenContent
                {
                    UserId = account.Id,
                    Email = account.Email,
                    User = account.UserName
                };

                AuthenticateResponse response = new AuthenticateResponse
                {
                    UserId = account.Id,
                    UserName = account.UserName,
                    Email = account.Email,
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


    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest model)
    {
        var account = accountManager.getByUserNameOrEmail(model.Username);
        if (account != null)
            throw new AppException("Username '" + model.Username + "' is already taken");
        account = Mapper.Map<Account>(model);
        account.Password = GeneralUtilities.ValidatePassword(model.Password);
        if (account.Password == null)
            throw new AppException("Minimum of different classes of characters in password is 3. Classes of characters: Lower Case, Upper Case, Digits, Special Characters.");
        accountManager.Add(account);
        return Ok(new { message = "Registration successful" });
    }

    [HttpGet]
    public IActionResult GetAccountById(Guid id)
    {
        return Ok(accountManager.GetById(id));
    }

    [HttpPut("{id}")]
    public IActionResult Update(UpdateRequest model)
    {
        var account = accountManager.getByUserNameOrEmail(model.Username);
        if (account == null || !GeneralUtilities.ComparePassword(model.Password, account.Password))
            throw new AppException("Username or password is incorrect");

        Mapper.Map(model, account);
        account.Password = GeneralUtilities.ValidatePassword(model.Password);
        if (account.Password == null)
            throw new AppException("Minimum of different classes of characters in password is 3. Classes of characters: Lower Case, Upper Case, Digits, Special Characters.");

        accountManager.Update(account);
        return Ok(new { message = "User updated successfully" });
    }
}