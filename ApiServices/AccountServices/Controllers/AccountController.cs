namespace AccountServices.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AccountServices.Utilities;
using AccountServices.Models.Api;
using OhMyMoney.DataCore.Entities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using AccountServices.Business.Interfaces;
using OhMyMoney.AuthMiddleware;
using OhMyMoney.AuthMiddleware.Models;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("Api/[controller]")]
public class AccountController : ControllerBase
{
    IUserManager UserManager;
    IMapper Mapper;
    readonly GoogleReCaptcha GoogleReCaptcha;
    readonly JwtTokenHandler JwtTokenHandler;

    public AccountController(IMapper mapper, IConfiguration configuration, IUserManager userManager, JwtTokenHandler jwtTokenHandler)
    {
        UserManager = userManager;
        Mapper = mapper;
        GoogleReCaptcha = new GoogleReCaptcha(configuration);
        JwtTokenHandler = jwtTokenHandler;
    }

    [AllowAnonymous]
    [HttpPost("Authenticate")]
    public async Task<IActionResult> Authenticate(AuthenticateRequest model)
    {
        var email = model.Param_1;
        var password = model.Param_2;
        var googleReCaptchaString = model.Param_3;
        var IP = HttpContext.Connection.RemoteIpAddress.ToString();

        var user = UserManager.getByUserOrEmail(email);
        var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (user == null || !GeneralUtilities.ComparePassword(password, user.Password))
            throw new AppException("Username or password is incorrect");
        else if (!user.IsActive)
        {
            throw new AppException("User Account is Deactivated Please Contact Admin");
        }
        //No validate recaptcha when is testing that's why ignore local host and Office Ip.
        else if (!await GoogleReCaptcha.Validate(googleReCaptchaString) && enviroment != "Development" && IP != "::1")
        {
            throw new AppException("Recaptcha validation failed");
        }
        var response = Mapper.Map<AuthenticateResponse>(user);
        var request = Mapper.Map<AuthenticationRequest>(model);
        response.Token = JwtTokenHandler.GenerateToken(request);
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

            var user = UserManager.getByUserOrEmail(userinfo.Email);
            if (user.Email == email)
            {
                var response = Mapper.Map<AuthenticateResponse>(user);
                var request = Mapper.Map<AuthenticationRequest>(model);
                response.Token = JwtTokenHandler.GenerateToken(request);
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
        var user = UserManager.getByUserOrEmail(model.Username);
        if (user != null)
            throw new AppException("Username '" + model.Username + "' is already taken");
        user = Mapper.Map<User>(model);
        user.Password = GeneralUtilities.ValidatePassword(model.Password);
        if (user.Password == null)
            throw new AppException("Minimum of different classes of characters in password is 3. Classes of characters: Lower Case, Upper Case, Digits, Special Characters.");
        UserManager.Add(user);
        return Ok(new { message = "Registration successful" });
    }

    [HttpPut("{id}")]
    public IActionResult Update(UpdateRequest model)
    {
        var user = UserManager.getByUserOrEmail(model.Username);
        if (user == null || !GeneralUtilities.ComparePassword(model.Password, user.Password))
            throw new AppException("Username or password is incorrect");

        Mapper.Map(model, user);
        user.Password = GeneralUtilities.ValidatePassword(model.Password);
        if (user.Password == null)
            throw new AppException("Minimum of different classes of characters in password is 3. Classes of characters: Lower Case, Upper Case, Digits, Special Characters.");

        UserManager.Update(user);
        return Ok(new { message = "User updated successfully" });
    }
}