using Famnances.AuthMiddleware;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class InflowsController : ControllerBase
    {
        IInflowManager _inflowManager;

        public InflowsController(IInflowManager inflowManager)
        {
            _inflowManager = inflowManager;
        }

        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit(Inflow inflow)
        {
            HttpContext.Items.TryGetValue("AccountId", out var accountId);
            inflow.UserId = Guid.Parse(accountId.ToString());
            //if (_fixedIncomeManager != null)
            //{
            //    FixedIncome fixedIncome = _fixedIncomeManager.GetById(inflow.FixedIncomeId.Value);
            //    inflow.Value = fixedIncome.Value;
            //    inflow.Description = fixedIncome.Description;
            //}
            inflow = _inflowManager.Add(inflow);
            return Ok(inflow);
        }
    }
}
