using Famnances.AuthMiddleware;
using Famnances.DataCore.Entities;
using FamnancesServices.Business;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [Route("Api/[controller]")]
    [ApiController]
    public class HomesController : ControllerBase
    {
        IHomeManager _homeManager;
        IHomeInvitationManager _homeInvitationManager;
        IAccountManager _accountManager;
        IUserManager _userManager;

        public HomesController(
            IHomeManager homeManager, 
            IHomeInvitationManager homeInvitationManager, 
            IAccountManager accountManager,
            IUserManager userManager
            )
        {
            _homeManager = homeManager;
            _homeInvitationManager = homeInvitationManager;
            _accountManager = accountManager;
            _userManager = userManager;
        }

        // GET: api/Homes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Home>> GetHome(Guid id)
        {
            return Ok(_homeManager.GetById(id));
        }

        [HttpGet("GetGuestRequests/{id}")]
        public async Task<ActionResult<List<HomeInvitation>>> GetGuestRequests(Guid id)
        {
            return Ok(_homeInvitationManager.GetRequests(id));
        }

        [HttpGet("GetInvitations")]
        public async Task<ActionResult<HomeInvitation>> GetInvitations()
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var account = _accountManager.GetById(userId);
            return Ok(_homeInvitationManager.GetInvitations(account.Email));
        }

        [HttpGet("AcceptInvitation/{invitationId}")]
        public async Task<ActionResult<HomeInvitation>> AcceptInvitation(Guid invitationId)
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var account = _accountManager.GetById(userId);

            var invitation = _homeInvitationManager.Accept(invitationId);

            var uGuest = _userManager.GetById(invitation.GuestId.Value);
            uGuest.HomeId = invitation.HomeId;
            _userManager.Update(uGuest);

            return Ok(invitation);
        }

        // POST: api/Homes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Invite")]
        public async Task<ActionResult<List<HomeInvitation>>> Invite(HomeInvitation homeInvitation)
        {
            HttpContext.Items.TryGetValue(Constants.USER, out var accountId);
            var userId = Guid.Parse(accountId.ToString());
            var account = _accountManager.GetById(userId);

            if (homeInvitation.IsInvitation)
                homeInvitation.HostId = account.Id;
            else
                homeInvitation.GuestId = account.Id;
            homeInvitation.InvitationDate = DateTime.Now;
            _homeInvitationManager.Add(homeInvitation);

            var invitations = _homeInvitationManager.GetInvitations(account.Email);
            var requests = _homeInvitationManager.GetRequests(homeInvitation.HomeId.Value);

            return Ok(homeInvitation.IsInvitation ? requests : invitations);
        }

        [HttpPost]
        public async Task<ActionResult<Home>> Create(Home Home)
        {
            _homeManager.Add(Home);
            return CreatedAtAction("GetHome", new { id = Home.Id }, Home);
        }

        // PUT: api/Homes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Home Home)
        {
            if (id != Home.Id)
            {
                return BadRequest();
            }

            try
            {
                _homeManager.Update(Home);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }


        // DELETE: api/Homes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHome(Guid id)
        {
            var Home = _homeManager.GetById(id);
            if (Home == null)
            {
                return NotFound();
            }

            _homeManager.Delete(Home);
            return NoContent();
        }
    }
}
