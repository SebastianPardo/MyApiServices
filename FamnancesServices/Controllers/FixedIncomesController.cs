using Famnances.Core.Security;
using Famnances.Core.Security.Authorization;
using Famnances.Core.Utils.Helpers;
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
    public class FixedIncomesController : ControllerBase
    {
        IFixedIncomeManager _fixedIncomeManager;
        IUtilitiesManager _utilitiesManager;
        IInflowManager _inflowManager;
        IIncomeDiscountManager _incomeDiscountManager;
        IExpensesBudgetManager _expensesBudgetManager;
        IOutflowManager _outflowManager;
        IFixedIncomeByDiscountManager _fixedIncomeByDiscountManager;

        public FixedIncomesController(
            IFixedIncomeManager fixedIncomeManager, 
            IInflowManager inflowManager,
            IUtilitiesManager utilitiesManager,
            IIncomeDiscountManager incomeDiscountManager,
            IExpensesBudgetManager expensesBudgetManager,
            IOutflowManager outflowManager,
            IFixedIncomeByDiscountManager fixedIncomeByDiscountManager)
        {
            _fixedIncomeManager = fixedIncomeManager;
            _inflowManager = inflowManager;
            _utilitiesManager = utilitiesManager;
            _incomeDiscountManager = incomeDiscountManager;
            _expensesBudgetManager = expensesBudgetManager;
            _outflowManager = outflowManager;
            _fixedIncomeByDiscountManager = fixedIncomeByDiscountManager;
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
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, FixedIncome fixedIncome)
        {
            if (id != fixedIncome.Id)
            {
                return BadRequest();
            }

            try
            {
                HttpContext.Items.TryGetValue(Constants.ACCOUNT_ID, out var accountId);
                fixedIncome.UserId = Guid.Parse(accountId.ToString());
                _fixedIncomeByDiscountManager.DeleteByFixedIncomme(fixedIncome.Id);
                _fixedIncomeManager.Update(fixedIncome);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }

        [HttpPost("Receive")]
        public async Task<IActionResult> Receive(Guid id)
        {
            try
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
                        TransactionDate = DateTimeEast.Now,
                        Value = income.Value,
                        UserId = userId,
                        InflowByDiscount = new List<InflowByDiscount>()
                    };


                    if (income.FixedIncomeByDiscount != null && income.FixedIncomeByDiscount.Count > 0)
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
                                    ExpenseBudgetId = _expensesBudgetManager.GetByType("DIS", inflow.UserId).First().Id,
                                    TransactionDate = inflow.TransactionDate,
                                };
                                _outflowManager.Add(outflow);
                            }
                        }
                    }
                    _inflowManager.Add(inflow);
                    income.LastAutomaticDateStamp = DateTimeEast.Now;
                    _fixedIncomeManager.Update(income);
                }
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Ok();
            }
        }
    }
}
