using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    [Route("Api/[controller]")]
    public class FinancialInstitutionsController : ControllerBase
    {
        IFinancialInstitutionManager _financialInstitutionManager;
        public FinancialInstitutionsController(IFinancialInstitutionManager financialInstitutionManager)
        {
            _financialInstitutionManager = financialInstitutionManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetFinancialInstitutions()
        {
            var financialInstitutions = _financialInstitutionManager.GetAll();
            return Ok(financialInstitutions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFinancialInstitution(Guid id)
        {
            var financialInstitution = _financialInstitutionManager.GetById(id);
            return Ok(financialInstitution);
        }

        [HttpGet("GetByCode/{code}")]
        public async Task<IActionResult> GetFinancialInstitutionByCode(string code)
        {
            var financialInstitution = _financialInstitutionManager.GetByCode(code);
            return Ok(financialInstitution);
        }
    }
}
