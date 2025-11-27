using Famnances.Core.Security;
using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using FamnancesServices.Business;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    [Route("Api/[controller]")]
    public class SavingPocketsController : Controller
    {
        ISavingsPocketManager _savingsPocketManager;
        public SavingPocketsController(ISavingsPocketManager savingsPocketManager)
        {
            _savingsPocketManager = savingsPocketManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SavingsPocket>>> GetPockets()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            return Ok(_savingsPocketManager.GetAllByUserId(userId));
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SavingsPocket>> GetPocket(Guid id)
        {
            return Ok(_savingsPocketManager.GetById(id));
        }

        [HttpGet("{id}/{from}/{to}")]
        public async Task<ActionResult<SavingsPocket>> GetFixedExpenseByDates(Guid id, DateTime from, DateTime to)
        {
            return Ok(_savingsPocketManager.GetCompleteByIdDates(id, from, to));
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SavingsPocket>> Create(SavingsPocket pocket)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            pocket.UserId = Guid.Parse(accountId.ToString());
            _savingsPocketManager.Add(pocket);
            return CreatedAtAction("GetPocket", new { id = pocket.Id }, pocket);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SavingsPocket pocket)
        {
            if (id != pocket.Id)
            {
                return BadRequest();
            }

            try
            {
                HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
                pocket.UserId = Guid.Parse(accountId.ToString());
                _savingsPocketManager.Update(pocket);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePocket(Guid id)
        {
            var pocket = _savingsPocketManager.GetById(id);
            if (pocket == null)
            {
                return NotFound();
            }

            _savingsPocketManager.Delete(pocket);
            return Ok();
        }
    }
}
