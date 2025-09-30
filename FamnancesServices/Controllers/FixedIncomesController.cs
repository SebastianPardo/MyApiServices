using Famnances.Core.Security;
using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using FamnancesServices.Business;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class FixedIncomesController : ControllerBase
    {
        IFixedIncomeManager _fixedIncomeManager;
        IUtilitiesManager _utilitiesManager;
        IInflowManager _inflowManager;
        public FixedIncomesController(
            IFixedIncomeManager fixedIncomeManager, 
            IInflowManager inflowManager,
            IUtilitiesManager utilitiesManager)
        {
            _fixedIncomeManager = fixedIncomeManager;
            _inflowManager = inflowManager;
            _utilitiesManager = utilitiesManager;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var fixedIncomes = _fixedIncomeManager.GetAllByUserId(userId);
            return Ok(fixedIncomes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var fixedIncomes = _fixedIncomeManager.GetById(userId, id);
            return Ok(fixedIncomes);
        }

        [HttpPost]
        public async Task<IActionResult> Add(FixedIncome entity)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            entity.UserId = Guid.Parse(accountId.ToString());
            entity = _fixedIncomeManager.Add(entity);
            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var fixedIncomes = _fixedIncomeManager.GetById(userId, id);
            if (fixedIncomes == null)
            {
                return NotFound();
            }
            _fixedIncomeManager.Delete(fixedIncomes);
            return NoContent();
        }

        [HttpPost("Receive")]
        public async Task<IActionResult> Receive(Guid id)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var income = _fixedIncomeManager.GetById(userId, id);
            var dates = _utilitiesManager.GetPeriodDates(income.PayablePeriodId, income.FirstPayDate.Day);
            if (income.LastAutomaticDateStamp == null || income.LastAutomaticDateStamp < dates.Item1)
            {
                Inflow inflow = new Inflow
                {
                    Description = income.Description,
                    TransactionDate = DateTime.Now,
                    Value = income.Value, 
                    UserId = userId
                };
                _inflowManager.Add(inflow);
                income.LastAutomaticDateStamp = DateTime.Now;
                _fixedIncomeManager.Update(income);
            }
            return Ok();
        }
    }
}
