using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamnancesServices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Api/[controller]")]
    public class CountriesController : ControllerBase
    {
        ICountryManager _countryManager;
        public CountriesController(ICountryManager countryManager)
        {
            _countryManager = countryManager;
        }
     
        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            List<Country> countries = _countryManager.GetAll();
            return Ok(countries);
        }
    }
}
