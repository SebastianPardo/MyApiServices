﻿namespace AccountServices.Controllers;

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
    IAccountManager accountManager;
    IMapper Mapper;
    readonly GoogleReCaptcha GoogleReCaptcha;
    readonly JwtTokenHandler JwtTokenHandler;

    public AccountController(IMapper mapper, IConfiguration configuration, IAccountManager accountManager, JwtTokenHandler jwtTokenHandler)
    {
        this.accountManager = accountManager;
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

        var account = accountManager.getByUserNameOrEmail(email);
        var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (account == null || !GeneralUtilities.ComparePassword(password, account.Password))
            throw new AppException("Username or password is incorrect");
        else if (!account.IsActive)
        {
            throw new AppException("User Account is Deactivated Please Contact Admin");
        }
        //No validate recaptcha when is testing that's why ignore local host and Office Ip.
        else if (!await GoogleReCaptcha.Validate(googleReCaptchaString) && enviroment != "Development" && IP != "::1")
        {
            throw new AppException("Recaptcha validation failed");
        }
        var response = Mapper.Map<AuthenticateResponse>(account);
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

            var account = accountManager.getByUserNameOrEmail(userinfo.Email);
            if (account.Email == email)
            {
                var response = Mapper.Map<AuthenticateResponse>(account);
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