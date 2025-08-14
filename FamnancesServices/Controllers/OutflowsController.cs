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
    public class OutflowsController : ControllerBase
    {
        IOutflowManager _outflowManager;
        public OutflowsController(IOutflowManager outflowManager)
        {
            _outflowManager = outflowManager;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<Outflow>>> GetOutflows()
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            return Ok(_outflowManager.GetAllByUserId(userId));
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<Outflow>> GetOutflow(Guid id)
        {
            return Ok(_outflowManager.GetById(id));
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Outflow outflow)
        {
            if (id != outflow.Id)
            {
                return BadRequest();
            }

            try
            {
                _outflowManager.Update(outflow);
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
        public async Task<ActionResult<User>> Create(Outflow outflow)
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            //outflow.UserId = Guid.Parse(accountId.ToString());
            _outflowManager.Add(outflow);
            return CreatedAtAction("GetBudget", new { id = outflow.Id }, outflow);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var outflow = _outflowManager.GetById(id);
            if (outflow == null)
            {
                return NotFound();
            }

            _outflowManager.Delete(outflow);
            return NoContent();
        }
    }
}
