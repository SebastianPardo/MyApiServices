using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using Famnances.Core.Security;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FamnancesServices.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class IncomeDiscountsController : ControllerBase
    {
        IIncomeDiscountManager _incomeDiscountManager;
        public IncomeDiscountsController(IIncomeDiscountManager incomeDiscountManager)
        {
            _incomeDiscountManager = incomeDiscountManager;
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public ActionResult<IEnumerable<IncomeDiscount>> Get()
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var discounts = _incomeDiscountManager.GetAllByUser(userId);
            return Ok(discounts);
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<IncomeDiscount>> Get(Guid id)
        {
            var discounts = _incomeDiscountManager.GetById(id);
            return Ok(discounts);
        }

        // POST api/<ValuesController>
        [HttpPost]
        public IActionResult Create(IncomeDiscount discount)
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            discount.UserId = Guid.Parse(accountId.ToString());
            _incomeDiscountManager.Add(discount);
            return CreatedAtAction("GetBudget", new { id = discount.Id }, discount);
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, IncomeDiscount discount)
        {
            if (id != discount.Id)
            {
                return BadRequest();
            }

            try
            {
                _incomeDiscountManager.Update(discount);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var budget = _incomeDiscountManager.GetById(id);
            if (budget == null)
            {
                return NotFound();
            }

            _incomeDiscountManager.Delete(budget);
            return NoContent();
        }
    }
}
