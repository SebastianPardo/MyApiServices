using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    [Route("Api/[controller]")]
    public class CitiesController : ControllerBase
    {
        ICityManager _cityManager;
        public CitiesController(ICityManager cityManager)
        {
            _cityManager = cityManager;
        }

        [HttpGet("GetCitiesByProvince/{provinceId}")]
        public async Task<IActionResult> GetCitiesByProvince(Guid provinceId)
        {
            List<City> cities = _cityManager.GetAllByProvince(provinceId);
            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(_cityManager.GetById(id));
        }
    }
}
