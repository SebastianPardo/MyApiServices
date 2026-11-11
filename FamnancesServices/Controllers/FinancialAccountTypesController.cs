using Famnances.Core.Security.Authorization;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    [Route("Api/[controller]")]
    public class FinancialAccountTypesController : ControllerBase
    {
        IFinancialAccountTypeManager _financialAccountTypeManager;

        public FinancialAccountTypesController(IFinancialAccountTypeManager financialAccountTypeManager)
        {
            _financialAccountTypeManager = financialAccountTypeManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetFinancialAccountTypes()
        {
            var financialAccountTypes = _financialAccountTypeManager.GetAll();
            return Ok(financialAccountTypes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFinancialAccountType(Guid id)
        {
            var financialAccountType = _financialAccountTypeManager.GetById(id);
            return Ok(financialAccountType);
        }

        [HttpGet("GetByCode/{code}")]
        public async Task<IActionResult> GetFinancialAccountTypeByCode(string code)
        {
            var financialAccountType = _financialAccountTypeManager.GetByCode(code);
            return Ok(financialAccountType);
        }
    }
}
