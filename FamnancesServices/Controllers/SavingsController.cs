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
    public class SavingsController : Controller
    {
        ISavingRecordManager _savingRecordManager;
        public SavingsController(ISavingRecordManager savingRecordManager)
        {
            _savingRecordManager = savingRecordManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SavingRecord>>> GetRecords()
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            return Ok(_savingRecordManager.GetAll(userId));
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SavingRecord>> GetRecord(Guid id)
        {
            return Ok(_savingRecordManager.GetById(id));
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SavingRecord>> Create(SavingRecord savingRecord)
        {
            _savingRecordManager.Add(savingRecord);
            return CreatedAtAction("GetRecord", new { id = savingRecord.Id }, savingRecord);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SavingRecord savingRecord)
        {
            if (id != savingRecord.Id)
            {
                return BadRequest();
            }

            try
            {
                _savingRecordManager.Update(savingRecord);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var savingRecord = _savingRecordManager.GetById(id);
            if (savingRecord == null)
            {
                return NotFound();
            }

            _savingRecordManager.Delete(savingRecord);
            return NoContent();
        }
    }
}
