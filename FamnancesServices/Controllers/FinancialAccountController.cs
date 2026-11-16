using Famnances.Core.Security;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialAccountController : ControllerBase
    {
        IFinancialAccountManager _financialAccountManager;

        public FinancialAccountController(IFinancialAccountManager financialAccountManager)
        {
            _financialAccountManager = financialAccountManager;
        }

        [HttpGet]
        public async Task<ActionResult<FixedExpense>> GetFinancialAccounts()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            return Ok(_financialAccountManager.GetByUserId(userId));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FixedExpense>> GetFinancialAccount(Guid id)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            return Ok(_financialAccountManager.GetById(userId, id));
        }

        [HttpGet("GetByFinancialAccountType/{financialAccountType}")]
        public async Task<ActionResult<FixedExpense>> GetFinancialAccountsByType(string financialAccountType)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            return Ok(_financialAccountManager.GetByFinancialAccountType(userId, financialAccountType));
        }

        [HttpPost]
        public async Task<ActionResult<FinancialAccount>> AddFinancialAccount(FinancialAccount financialAccount)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            financialAccount.UserId = userId;
            return Ok(_financialAccountManager.Add(financialAccount));
        }
    }
}
