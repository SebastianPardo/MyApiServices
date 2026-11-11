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
            return Ok(_financialAccountManager.GetAll(userId));
        }
    }
}
