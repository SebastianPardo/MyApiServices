using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class ProvincesController : ControllerBase
    {
        IProvinceManager _provinceManager;
        public ProvincesController(IProvinceManager provinceManager)
        {
            _provinceManager = provinceManager;
         }

        [HttpGet("GetProvincesByCountry/{countryId}")]
        public async Task<IActionResult> GetProvincesByCountry(Guid countryId)
        {
            List<Province> provinces = _provinceManager.GetAllByCountry(countryId);
            return Ok(provinces);
        }
    }
}
