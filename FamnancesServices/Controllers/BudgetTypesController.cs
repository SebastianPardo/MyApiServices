using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Migrations;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [Route("Api/[controller]")]
    [ApiController]
    public class BudgetTypesController : ControllerBase
    {
        IBudgetTypeManager _budgetTypeManager;

        public BudgetTypesController(IBudgetTypeManager budgetTypeManager)
        {
            _budgetTypeManager = budgetTypeManager;
        }

        [HttpGet("GetByCode/{code}")]
        public async Task<ActionResult<BudgetTypes>> GetByCode(string code)
        {
            return Ok(_budgetTypeManager.GetByCode(code));
        }
    }
}
