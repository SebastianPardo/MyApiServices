using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    [Route("Api/[controller]")]
    public class ErrorLogController : Controller
    {
        IErrorLogManager _errorLogManager;
        public ErrorLogController(IErrorLogManager errorLogManager)
        {
            _errorLogManager = errorLogManager;
        }


        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ErrorLog>> GetErrorLog(Guid id)
        {
            return Ok(_errorLogManager.GetById(id));
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ErrorLog>> Create(ErrorLog error)
        {
            _errorLogManager.Add(error);
            return CreatedAtAction("GetErrorLog", new { id = error.Id }, error);
        }

    }
}
