using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class SocialMediaController : ControllerBase
    {
        ISocialMediaManager _socialMediaManager;
        public SocialMediaController(ISocialMediaManager socialMediaManager)
        {
            _socialMediaManager = socialMediaManager;
        }
    }
}
