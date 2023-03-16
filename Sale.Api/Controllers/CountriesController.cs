using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Shared.Entities;

namespace Sale.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly DataContext _context;

        public CountriesController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult>Get()
        {
            return Ok(await _context.countries.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult>Post(Country country)
        {
            _context.Add(country);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
