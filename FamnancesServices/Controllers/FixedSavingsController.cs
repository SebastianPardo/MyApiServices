using Famnances.Core.Security;
using Famnances.DataCore.Entities;
using FamnancesServices.Business;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FamnancesServices.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class FixedSavingsController : ControllerBase
    {
        IFixedSavingManager _fixedSavingManager;
        public FixedSavingsController(IFixedSavingManager fixedSavingManager)
        {
            _fixedSavingManager = fixedSavingManager;
        }
        // GET: api/<FixedSavingsController>
        [HttpGet]
        public IEnumerable<FixedSaving> Get()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            return _fixedSavingManager.GetAllByUserId(userId);
        }

        // GET api/<FixedSavingsController>/5
        [HttpGet("{id}")]
        public FixedSaving Get(Guid id)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());

            return _fixedSavingManager.GetById(userId, id);
        }

        // POST api/<FixedSavingsController>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, FixedSaving fixedSaving)
        {
            if (id != fixedSaving.Id)
            {
                return BadRequest();
            }

            try
            {
                _fixedSavingManager.Update(fixedSaving);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }

        // PUT api/<FixedSavingsController>/5

        [HttpPost]
        public async Task<ActionResult<FixedExpense>> Create(FixedSaving fixedSaving)
        {
            _fixedSavingManager.Add(fixedSaving);
            return CreatedAtAction("Get", new { id = fixedSaving.Id }, fixedSaving);
        }

        // DELETE api/<FixedSavingsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var fixedSaving = _fixedSavingManager.GetById(userId, id);
            if (fixedSaving == null)
            {
                return NotFound();
            }

            _fixedSavingManager.Delete(fixedSaving);
            return Ok();
        }
    }
}
