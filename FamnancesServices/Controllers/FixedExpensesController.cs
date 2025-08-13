using Famnances.AuthMiddleware;
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

        public FixedExpensesController(IFixedExpenseManager fixedExpenseManager)
        {
            _fixedExpenseManager = fixedExpenseManager;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<FixedExpense>>> GetFixedExpenses()
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            return Ok(_fixedExpenseManager.GetAllByUserId(userId));
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
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
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
    }
}
