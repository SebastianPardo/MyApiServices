using Famnances.DataCore.Entities;
using FamnancesServices.Business;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    public class IncomeDiscountsController : ControllerBase
    {
        IIncomeDiscountManager _incomeDiscountManager;
        IAutomaticDiscountManager _automaticDiscountManager;

        public IncomeDiscountsController(IncomeDiscountManager incomeDiscountManager, IAutomaticDiscountManager automaticDiscountManager)
        {
            _automaticDiscountManager = automaticDiscountManager;
            _incomeDiscountManager = incomeDiscountManager;
        }


        [HttpGet]
        public async Task<IActionResult> GetIncomeDiscounts()
        {
            HttpContext.Items.TryGetValue("AccountId", out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            IEnumerable<IncomeDiscount> entity = _incomeDiscountManager.GetAllByUser(userId);
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> AddIncomeDiscount(IncomeDiscount entity)
        {
            HttpContext.Items.TryGetValue("AccountId", out var accountId);
            entity.UserId = Guid.Parse(accountId.ToString());
            entity = _incomeDiscountManager.Add(entity);
            return Ok(entity);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateIncomeDiscount(IncomeDiscount entity)
        {
            HttpContext.Items.TryGetValue("AccountId", out var accountId);
            entity.UserId = Guid.Parse(accountId.ToString());
            _incomeDiscountManager.Update(entity);
            return Ok(entity);
        }
    }
}
