using Famnances.AuthMiddleware;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class BudgetsController : ControllerBase
    {
        IExpensesBudgetManager _expensesBudgetManager;
        public BudgetsController(IExpensesBudgetManager expensesBudgetManager)
        {
            _expensesBudgetManager = expensesBudgetManager;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<ExpensesBudget>>> GetBudgets()
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            return Ok(_expensesBudgetManager.GetAllByUserId(userId));
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<ExpensesBudget>> GetBudget(Guid id)
        {
            return Ok(_expensesBudgetManager.GetById(id));
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
                _expensesBudgetManager.Update(budget);
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
        public async Task<ActionResult<ExpensesBudget>> Create(ExpensesBudget budget)
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
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
            return NoContent();
        }
    }
}
