namespace AccountServices.Controllers;

using AccountServices.Business.Interfaces;
using AccountServices.Models.ApiModels;
using AutoMapper;
using Famnances.AuthMiddleware;
using Famnances.AuthMiddleware.Interfaces;
using Famnances.DataCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using AuthorizeAttribute = Famnances.AuthMiddleware.AuthorizeAttribute;

[Authorize]
[ApiController]
[Route("Api/[controller]")]
public class AccountController : ControllerBase
{
    IMapper Mapper;
    IAccountService _accountService;
    IUtilityService _utilityService;

    public AccountController(IMapper mapper, IConfiguration configuration, IAccountService accountService, IUtilityService utilityService, ITokenHandler tokenHandler)
    {
        _accountService = accountService;
        _utilityService = utilityService;
        Mapper = mapper;
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
        var account = _accountService.GetType(accountId);
        return Ok(account);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest model)
    {
        var user = _accountService.getByUserNameOrEmail(model.Username);
        if (user != null)
            throw new AppException("Username '" + model.Username + "' is already taken");
        user = Mapper.Map<Account>(model);
        user.Password = _utilityService.ValidatePassword(model.Password);
        if (user.Password == null)
            throw new AppException("Minimum of different classes of characters in password is 3. Classes of characters: Lower Case, Upper Case, Digits, Special Characters.");
        _accountService.Add(user);
        return Ok(new { message = "Registration successful" });
    }

    [HttpPut("{id}")]
    public IActionResult Update(UpdateRequest model)
    {
        var user = _accountService.getByUserNameOrEmail(model.Username);
        if (user == null || !_utilityService.ComparePassword(model.Password, user.Password))
            throw new AppException("Username or password is incorrect");

        Mapper.Map(model, user);
        user.Password = _utilityService.ValidatePassword(model.Password);
        if (user.Password == null)
            throw new AppException("Minimum of different classes of characters in password is 3. Classes of characters: Lower Case, Upper Case, Digits, Special Characters.");

        _accountService.Update(user);
        return Ok(new { message = "User updated successfully" });
    }
}