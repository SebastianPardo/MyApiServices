using Famnances.Core.Security;
using Famnances.Core.Security.Authorization;
using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    [Route("Api/[controller]")]
    public class InflowsController : ControllerBase
    {
        IInflowManager _inflowManager;
        IIncomeDiscountManager _incomeDiscountManager;
        IExpensesBudgetManager _expensesBudgetManager;
        IOutflowManager _outflowManager;
        IInflowByDiscountManager _inflowByDiscountManager;

        public InflowsController(
            IInflowManager inflowManager,
            IIncomeDiscountManager incomeDiscountManager,
            IExpensesBudgetManager expensesBudgetManager,
            IOutflowManager outflowManager,
            IInflowByDiscountManager inflowByDiscountManager)
        {
            _inflowManager = inflowManager;
            _incomeDiscountManager = incomeDiscountManager;
            _expensesBudgetManager = expensesBudgetManager;
            _outflowManager = outflowManager;
            _inflowByDiscountManager = inflowByDiscountManager;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inflow>>> GetInflows(DateTime? startDate = null, DateTime? endDate = null)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            Guid userId = Guid.Parse(accountId.ToString());
            startDate = startDate ?? DateTimeEast.Now.AddDays(-15);
            endDate = endDate ?? DateTimeEast.Now;
            return Ok(_inflowManager.GetAllByPeriod(startDate.Value, endDate.Value, userId));
        }

        [HttpGet("{from}/{to}")]
        public async Task<ActionResult<IEnumerable<Inflow>>> GetInflowByDates(DateTime from, DateTime to)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            return Ok(_inflowManager.GetAllByPeriod(from, to, userId));
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
        public async Task<IActionResult> Update(Guid id, IncomeTransactionModel inflow)
        {
            if (id != inflow.Income.Id)
            {
                return BadRequest();
            }

            try
            {
                var oldDiscounts = _inflowManager.GetDiscountsByInflow(id);
                var oldInflow = _inflowManager.GetById(id);
                foreach (var oldDiscount in oldDiscounts)
                {
                    decimal discountValue = oldDiscount.IncomeDiscount.IsPercentage ? oldInflow.Value * oldDiscount.IncomeDiscount.Value / 100 : oldDiscount.IncomeDiscount.Value;

                    if (!oldDiscount.IncomeDiscount.IsPrediscount)                    
                    {
                        Inflow discountRolledBack = new Inflow
                        {
                            Id = Guid.NewGuid(),
                            Value = discountValue,
                            DateTimeStamp = DateTimeEast.Now,
                            Description = $"{oldDiscount.IncomeDiscount.Description} - Discount Rolled back",
                            TransactionDate = inflow.Income.TransactionDate,
                        };
                        _inflowManager.Add(discountRolledBack);
                        _inflowByDiscountManager.Delete(oldDiscount);
                    }
                }

                Inflow entity = inflow.Income;
                entity.InflowByDiscount = new List<InflowByDiscount>();
                if (inflow.SelectedIncomeDiscountIds != null || inflow.SelectedIncomeDiscountIds.Count > 0)
                {
                    foreach (var discountId in inflow.SelectedIncomeDiscountIds)
                    {
                        entity.InflowByDiscount.Add(new InflowByDiscount { IncomeDiscountId = discountId });
                        IncomeDiscount discount = _incomeDiscountManager.GetById(discountId);
                        decimal discountValue = discount.IsPercentage ? entity.Value * discount.Value / 100 : discount.Value;

                        if (discount.IsPrediscount)
                        {
                            entity.Value = entity.Value - discountValue;
                        }
                        else
                        {
                            Outflow outflow = new Outflow
                            {
                                Id = Guid.NewGuid(),
                                Value = discountValue,
                                DateTimeStamp = DateTimeEast.Now,
                                Description = $"{discount.Description} - Discount",
                                ExpenseBudgetId = _expensesBudgetManager.GetByType("DIS", entity.UserId).First().Id,
                                TransactionDate = entity.TransactionDate,
                            };
                            _outflowManager.Add(outflow);
                        }
                    }
                }
                _inflowManager.Update(inflow.Income);
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
        public async Task<ActionResult<Inflow>> Create(IncomeTransactionModel inflow)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);

            Inflow entity = inflow.Income;
            entity.InflowByDiscount = new List<InflowByDiscount>();
            entity.UserId = Guid.Parse(accountId.ToString());

            if (inflow.SelectedIncomeDiscountIds != null && inflow.SelectedIncomeDiscountIds.Count > 0)
            {
                foreach (var discountId in inflow.SelectedIncomeDiscountIds)
                {
                    entity.InflowByDiscount.Add(new InflowByDiscount { IncomeDiscountId = discountId });
                    IncomeDiscount discount = _incomeDiscountManager.GetById(discountId);
                    decimal discountValue = discount.IsPercentage ? entity.Value * discount.Value / 100 : discount.Value;

                    if (discount.IsPrediscount)
                    {
                        entity.Value = entity.Value - discountValue;
                    }
                    else
                    {
                        Outflow outflow = new Outflow
                        {
                            Id = Guid.NewGuid(),
                            Value = discountValue,
                            DateTimeStamp = DateTimeEast.Now,
                            Description = $"{discount.Description} - Discount",
                            ExpenseBudgetId = _expensesBudgetManager.GetByType("DIS", entity.UserId).First().Id,
                            TransactionDate = entity.TransactionDate,
                        };
                        _outflowManager.Add(outflow);
                    }
                }
            }
            _inflowManager.Add(entity);
            return CreatedAtAction("GetInflow", new { id = entity.Id }, inflow);
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
            return Ok();
        }
    }
}
