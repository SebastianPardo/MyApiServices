using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Famnances.Core.Security;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [Route("Api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        IUserManager _userManager;

        public UsersController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(_userManager.GetAll());
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            return Ok(_userManager.GetById(id));
        }

        [HttpGet("Search/{name}")]
        public async Task<ActionResult<IEnumerable<User>>> Search(string name)
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var user = _userManager.GetById(userId);
            return Ok(_userManager.Search(name, user.HomeAdministrator));
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            try
            {
                _userManager.Update(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            _userManager.Add(user);
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = _userManager.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            _userManager.Delete(user);
            return NoContent();
        }
    }
}
