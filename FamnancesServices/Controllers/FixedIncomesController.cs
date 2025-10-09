using Famnances.Core.Security;
using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using FamnancesServices.Business;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class FixedIncomesController : ControllerBase
    {
        IFixedIncomeManager _fixedIncomeManager;
        IUtilitiesManager _utilitiesManager;
        IInflowManager _inflowManager;
        IIncomeDiscountManager _incomeDiscountManager;
        IExpensesBudgetManager _expensesBudgetManager;
        IOutflowManager _outflowManager;

        public FixedIncomesController(
            IFixedIncomeManager fixedIncomeManager, 
            IInflowManager inflowManager,
            IUtilitiesManager utilitiesManager,
            IIncomeDiscountManager incomeDiscountManager,
            IExpensesBudgetManager expensesBudgetManager,
            IOutflowManager outflowManager)
        {
            _fixedIncomeManager = fixedIncomeManager;
            _inflowManager = inflowManager;
            _utilitiesManager = utilitiesManager;
            _incomeDiscountManager = incomeDiscountManager;
            _expensesBudgetManager = expensesBudgetManager;
            _outflowManager = outflowManager;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var fixedIncomes = _fixedIncomeManager.GetAllByUserId(userId);
            return Ok(fixedIncomes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var fixedIncomes = _fixedIncomeManager.GetById(userId, id);
            return Ok(fixedIncomes);
        }

        [HttpPost]
        public async Task<IActionResult> Add(FixedIncome entity)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            entity.UserId = Guid.Parse(accountId.ToString());
            entity = _fixedIncomeManager.Add(entity);
            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var fixedIncomes = _fixedIncomeManager.GetById(userId, id);
            if (fixedIncomes == null)
            {
                return NotFound();
            }
            _fixedIncomeManager.Delete(fixedIncomes);
            return NoContent();
        }

        [HttpPost("Receive")]
        public async Task<IActionResult> Receive(Guid id)
        {
            HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var income = _fixedIncomeManager.GetById(userId, id);
            var dates = _utilitiesManager.GetPeriodDates(income.PayablePeriodId, income.FirstPayDate.Day);
            if (income.LastAutomaticDateStamp == null || income.LastAutomaticDateStamp < dates.Item1)
            {
                Inflow inflow = new Inflow
                {
                    Description = income.Description,
                    TransactionDate = DateTime.Now,
                    Value = income.Value,
                    UserId = userId,
                    InflowByDiscount = new List<InflowByDiscount>()
                };

                
                if (income.FixedIncomeByDiscount != null || income.FixedIncomeByDiscount.Count > 0)
                {
                    foreach (var fixedIncomeByDiscount in income.FixedIncomeByDiscount)
                    {
                        inflow.InflowByDiscount.Add(new InflowByDiscount { IncomeDiscountId = fixedIncomeByDiscount.IncomeDiscountId });
                        IncomeDiscount discount = _incomeDiscountManager.GetById(fixedIncomeByDiscount.IncomeDiscountId);
                        decimal discountValue = discount.IsPercentage ? inflow.Value * discount.Value / 100 : discount.Value;

                        if (discount.IsPrediscount)
                        {
                            inflow.Value = inflow.Value - discountValue;
                        }
                        else
                        {
                            Outflow outflow = new Outflow
                            {
                                Id = Guid.NewGuid(),
                                Value = discountValue,
                                DateTimeStamp = DateTimeEast.Now,
                                Description = $"{discount.Description} - Discount",
                                ExpenseBudgetId = _expensesBudgetManager.GetByType("", inflow.UserId).First().Id,
                                TransactionDate = inflow.TransactionDate,
                            };
                            _outflowManager.Add(outflow);
                        }
                    }
                }
                _inflowManager.Add(inflow);


                _inflowManager.Add(inflow);
                income.LastAutomaticDateStamp = DateTime.Now;
                _fixedIncomeManager.Update(income);
            }
            return Ok();
        }
    }
}
