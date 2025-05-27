using Famnances.AuthMiddleware;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class ManagementController : ControllerBase
    {
        IPeriodManager _periodManager;
        ICountryManager _countryManager;
        IProvinceManager _provinceManager;
        ICityManager _cityManager;
        public ManagementController(
            IPeriodManager periodManager,
            ICountryManager countryManager,
            IProvinceManager provinceManager,
            ICityManager cityManager
            )
        {
            _periodManager = periodManager;
            _countryManager = countryManager;
            _provinceManager = provinceManager;
            _cityManager = cityManager;
        }

        [HttpGet("GetPeriods")]
        public async Task<IActionResult> GetPeriods()
        {
            var periods = _periodManager.GetAll();
            return Ok(periods);
        }

        [HttpGet("GetCountries")]
        public async Task<IActionResult> GetCountries()
        {
            List<Country> countries = _countryManager.GetAll();
            return Ok(countries);
        }

        [HttpGet("GetProvincesByCountry/{countryId}")]
        public async Task<IActionResult> GetProvincesByCountry(Guid countryId)
        {
            List<Province> provinces = _provinceManager.GetAllByCountry(countryId);
            return Ok(provinces);
        }

        [HttpGet("GetCitiesByProvince/{provinceId}")]
        public async Task<IActionResult> GetCitiesByProvince(Guid provinceId)
        {
            List<City> cities = _cityManager.GetAllByProvince(provinceId);
            return Ok(cities);
        }
    }
}
