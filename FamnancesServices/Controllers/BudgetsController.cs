using Famnances.AuthMiddleware;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class BudgetsController : ControllerBase
    {
        IExpensesBudgetManager _expensesBudgetManager;
        public BudgetsController(IExpensesBudgetManager expensesBudgetManager)
        {
            _expensesBudgetManager = expensesBudgetManager;
        }
    }
}
