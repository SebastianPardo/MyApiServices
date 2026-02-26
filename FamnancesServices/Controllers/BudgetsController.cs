using Famnances.Core.Security;
using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using FamnancesServices.Business;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    [Route("Api/[controller]")]
    public class BudgetsController : ControllerBase
    {
        IExpensesBudgetManager _expensesBudgetManager;
        ITotalsByPeriodManager _totalsByPeriodManager;
        IExpensesBudgetByPeriodManager _expensesBudgetByPeriodManager;

        public BudgetsController(
            IExpensesBudgetManager expensesBudgetManager,
            ITotalsByPeriodManager totalsByPeriodManager,
            IExpensesBudgetByPeriodManager expensesBudgetByPeriodManager
            )
        {
            _expensesBudgetManager = expensesBudgetManager;
            _totalsByPeriodManager = totalsByPeriodManager;
            _expensesBudgetByPeriodManager = expensesBudgetByPeriodManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpensesBudget>>> GetBudgets()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            return Ok(_expensesBudgetManager.GetAllByUserId(userId));
        }

        [HttpGet("GetEditables")]
        public async Task<ActionResult<IEnumerable<ExpensesBudget>>> GetEditables()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            return Ok(_expensesBudgetManager.GetAllByUserIdToEdit(userId));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpensesBudget>> GetBudget(Guid id)
        {
            return Ok(_expensesBudgetManager.GetById(id));
        }

        [HttpGet("GetSummary")]
        public async Task<ActionResult<List<SummaryBudgetModel>>> GetSummary()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());

            var totalsByPeriod = _totalsByPeriodManager.GetMostRecent(userId);
            var summary = _expensesBudgetByPeriodManager.ExpensesBudgetsSummary(userId, totalsByPeriod.PeriodDateStart.AddDays(1));

            return Ok(summary.Where(e => e.UserId == userId));
        }

        [HttpGet("{id}/{from}/{to}")]
        public async Task<ActionResult<ExpensesBudget>> GetFixedExpenseByDates(Guid id, DateTime from, DateTime to)
        {
            return Ok(_expensesBudgetManager.GetCompleteByIdDates(id, from, to));
        }

        [HttpGet("GetByType/{type}")]
        public async Task<ActionResult<FixedExpense>> GetByType(string type)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            return Ok(_expensesBudgetManager.GetByType(type, userId));
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ExpensesBudget budget)
        {
            if (id != budget.Id)
            {
                return BadRequest();
            }

            try
            {
                HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
                var userId = Guid.Parse(accountId.ToString());
                budget.UserId = userId;
                _expensesBudgetManager.Update(budget);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ExpensesBudget>> Create(ExpensesBudget budget)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            budget.UserId = Guid.Parse(accountId.ToString());
            _expensesBudgetManager.Add(budget);
            return CreatedAtAction("GetBudget", new { id = budget.Id }, budget);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var budget = _expensesBudgetManager.GetById(id);
            if (budget == null)
            {
                return NotFound();
            }

            _expensesBudgetManager.Delete(budget);
            return Ok();
        }
    }
}
