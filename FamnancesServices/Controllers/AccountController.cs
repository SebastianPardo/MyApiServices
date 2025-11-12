namespace FamnancesServices.Controllers;

using Famnances.Core.Errors;
using Famnances.Core.Security.Authorization;
using Famnances.Core.Security.Services.Interfaces;
using Famnances.Core.Utils.Services.Interface;
using FamnancesServices.Business.Interfaces;
using FamnancesServices.Models;
using Microsoft.AspNetCore.Mvc;

[ServiceFilter(typeof(AuthorizeAttribute))]
[ApiController]
[Route("Api/[controller]")]
public class AccountController : ControllerBase
{
    IAccountManager _accountService;
    IPasswordService _passwordService;

    public AccountController(IConfiguration configuration, IAccountManager accountService, IPasswordService passwordService, ITokenHandler tokenHandler)
    {
        _accountService = accountService;
        _passwordService = passwordService;
    }    

    [HttpGet("{accountId}")]
    public IActionResult GetAccount(Guid accountId)
    {
        var account = _accountService.GetById(accountId);
        return Ok(account);
    }
    [HttpGet("Type/{accountId}")]
    public IActionResult GetAccountType(Guid accountId)
    {
        var account = _accountService.GetType();
        return Ok(account);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest model)
    {
        var user = _accountService.getByUserNameOrEmail(model.Username);
        if (user != null)
            throw new AppException("Username '" + model.Username + "' is already taken");
        //user = model;
        user.Password = _passwordService.Validate(model.Password);
        if (user.Password == null)
            throw new AppException("Minimum of different classes of characters in password is 3. Classes of characters: Lower Case, Upper Case, Digits, Special Characters.");
        _accountService.Add(user);
        return Ok(new { message = "Registration successful" });
    }

    [HttpPut("{id}")]
    public IActionResult Update(UpdateRequest model)
    {
        var user = _accountService.getByUserNameOrEmail(model.Username);
        if (user == null || !_passwordService.Compare(model.Password, user.Password))
            throw new AppException("Username or password is incorrect");

        //Mapper.Map(model, user);
        user.Password = _passwordService.Validate(model.Password);
        if (user.Password == null)
            throw new AppException("Minimum of different classes of characters in password is 3. Classes of characters: Lower Case, Upper Case, Digits, Special Characters.");

        _accountService.Update(user);
        return Ok(new { message = "User updated successfully" });
    }
}