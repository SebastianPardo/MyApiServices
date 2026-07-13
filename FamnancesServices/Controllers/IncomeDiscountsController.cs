using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using Famnances.Core.Security;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FamnancesServices.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    [Route("Api/[controller]")]
    public class IncomeDiscountsController : ControllerBase
    {
        IIncomeDiscountManager _incomeDiscountManager;
        IInflowManager _inflowManager;
        public IncomeDiscountsController(IIncomeDiscountManager incomeDiscountManager, IInflowManager inflowManager)
        {
            _incomeDiscountManager = incomeDiscountManager;
            _inflowManager = inflowManager;
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public ActionResult<IEnumerable<IncomeDiscount>> Get()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var discounts = _incomeDiscountManager.GetAllByUser(userId);
            return Ok(discounts);
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public ActionResult<IncomeDiscount> Get(Guid id)
        {
            var discounts = _incomeDiscountManager.GetById(id);
            return Ok(discounts);
        }

        // POST api/<ValuesController>
        [HttpPost]
        public IActionResult Create(IncomeDiscount discount)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            discount.UserId = Guid.Parse(accountId.ToString());
            discount = _incomeDiscountManager.Add(discount);
            return CreatedAtAction("Get", new { id = discount.Id }, discount);
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
                HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
                discount.UserId = Guid.Parse(accountId.ToString());
                _incomeDiscountManager.Update(discount);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var discount = _incomeDiscountManager.GetById(id);
            if (discount == null)
            {
                return NotFound();
            }

            var inflowByDiscount  = _inflowManager.GetByDiscountId(discount.Id);
            if (inflowByDiscount != null && inflowByDiscount.Count > 0)
            {
                discount.Active = false;
                _incomeDiscountManager.Update(discount);
            }
            else
            {
                _incomeDiscountManager.Delete(discount);
            }
            return Ok();
        }
    }
}
