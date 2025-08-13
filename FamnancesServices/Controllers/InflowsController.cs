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
    public class InflowsController : ControllerBase
    {
        IInflowManager _inflowManager;
        Guid userId;

        public InflowsController(IInflowManager inflowManager,  IUserManager userManager)
        {
            _inflowManager = inflowManager;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inflow>>> GetInflows(DateTime? startDate = null, DateTime? endDate = null)
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            userId = Guid.Parse(accountId.ToString());
            startDate = startDate ?? DateTime.Now.AddDays(-15);
            endDate = endDate ?? DateTime.Now;
            return Ok(_inflowManager.GetAllByPeriod(startDate.Value, endDate.Value, userId));
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inflow>> GetInflow(Guid id)
        {
            return Ok(_inflowManager.GetById(id));
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Inflow inflow)
        {
            if (id != inflow.Id)
            {
                return BadRequest();
            }

            try
            {
                _inflowManager.Update(inflow);
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
        public async Task<ActionResult<Inflow>> Create(Inflow inflow)
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            inflow.UserId = Guid.Parse(accountId.ToString());
            _inflowManager.Add(inflow);
            return CreatedAtAction("GetInflow", new { id = inflow.Id }, inflow);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var inflow = _inflowManager.GetById(id);
            if (inflow == null)
            {
                return NotFound();
            }

            _inflowManager.Delete(inflow);
            return NoContent();
        }
    }
}
