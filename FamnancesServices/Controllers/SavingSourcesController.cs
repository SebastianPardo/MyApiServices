using Famnances.Core.Security;
using Famnances.Core.Security.Authorization;
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
    public class SavingSourcesController : Controller
    {
        ISavingSourceManager _savingSourceManager;
        public SavingSourcesController(ISavingSourceManager savingSourceManager)
        {
            _savingSourceManager = savingSourceManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SavingSource>>> Get()
        {
            return Ok(_savingSourceManager.GetAll());
        }

        // GET: api/Users/5
        [HttpGet("{code}")]
        public async Task<ActionResult<SavingsPocket>> Get(string code)
        {
            return Ok(_savingSourceManager.GetByCode(code));
        }
    }
}
