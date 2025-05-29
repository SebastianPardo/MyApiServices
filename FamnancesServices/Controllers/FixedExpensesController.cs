using Famnances.AuthMiddleware;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class FixedExpensesController : ControllerBase
    {
        IFixedExpenseManager _fixedExpenseManager;

        public FixedExpensesController(IFixedExpenseManager fixedExpenseManager)
        {
            _fixedExpenseManager = fixedExpenseManager;
        }
    }
}
