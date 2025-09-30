using Famnances.Core.Security;
using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using FamnancesServices.Business;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class FixedExpensesController : ControllerBase
    {
        IFixedExpenseManager _fixedExpenseManager;
        IExpensesBudgetManager _expensesBudgetManager;
        IOutflowManager _outflowManager;
        ITotalsByPeriodManager _totalsByPeriodManager;
        IUtilitiesManager _utilitiesManager;

        public FixedExpensesController(
            IFixedExpenseManager fixedExpenseManager,
            IExpensesBudgetManager expensesBudgetManager,
            IOutflowManager outflowManager,
            ITotalsByPeriodManager totalsByPeriodManager,
            IUtilitiesManager utilitiesManager
            )
        {
            _fixedExpenseManager = fixedExpenseManager;
            _expensesBudgetManager = expensesBudgetManager;
            _outflowManager = outflowManager;
            _totalsByPeriodManager = totalsByPeriodManager;
            _utilitiesManager = utilitiesManager;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<FixedExpense>>> GetFixedExpenses()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var expeneses = _fixedExpenseManager.GetAllByUserId(userId);
            var totalsByPeriod = _totalsByPeriodManager.GetByCurrentDay(userId);

            foreach(var expense in expeneses)
            {
                if (!(expense.LastAutomaticDateStamp >= totalsByPeriod.PeriodDateStart && expense.LastAutomaticDateStamp <= totalsByPeriod.PeriodDateEnd))
                {
                    expense.LastAutomaticDateStamp = null;
                }
            }

            return Ok(expeneses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FixedExpense>> GetFixedExpense(Guid id)
        {
            return Ok(_fixedExpenseManager.GetById(id));
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, FixedExpense fixedExpense)
        {
            if (id != fixedExpense.Id)
            {
                return BadRequest();
            }

            try
            {
                HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
                fixedExpense.UserId = Guid.Parse(accountId.ToString());
                _fixedExpenseManager.Update(fixedExpense);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> Create(FixedExpense fixedExpense)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            fixedExpense.UserId = Guid.Parse(accountId.ToString());
            _fixedExpenseManager.Add(fixedExpense);
            return CreatedAtAction("GetBudget", new { id = fixedExpense.Id }, fixedExpense);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var budget = _fixedExpenseManager.GetById(id);
            if (budget == null)
            {
                return NotFound();
            }

            _fixedExpenseManager.Delete(budget);
            return NoContent();
        }

        [HttpPost("Pay")]
        public async Task<IActionResult> Pay(Guid id)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var expense = _fixedExpenseManager.GetById(id);
            var dates = _utilitiesManager.GetPeriodDates(expense.PeriodId, expense.StartDate.Day);
            if (expense.LastAutomaticDateStamp == null || expense.LastAutomaticDateStamp < dates.Item1)
            {
                var budget = _expensesBudgetManager.GetByType("FIX", userId);
                Outflow outflow = new Outflow
                {
                    Description = expense.Name,
                    ExpenseBudgetId = budget.First().Id,
                    TransactionDate = DateTime.Now,
                    Value = expense.Value,
                };
                _outflowManager.Add(outflow);
                expense.LastAutomaticDateStamp = DateTime.Now;
                _fixedExpenseManager.Update(expense);
            }
            return Ok();
        }
    }
}
