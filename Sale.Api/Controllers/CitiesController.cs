using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Shared.Entities;

namespace Sale.Api.Controllers
{
    [Route("api/[cities]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly DataContext _context;

        public CitiesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult>GetAsync()
        {
            return Ok(await _context.Cities.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync(int id )
        {
            var city =await _context.Cities.FirstOrDefaultAsync(x=>x.Id== id);
            if(city == null) { return NotFound(); }
            return Ok(city);
        }
        [HttpPost]
        public async Task<IActionResult>PostAsync(City city)
        {
            try
            {
                _context.Add(city);
                _context.SaveChanges();
                return Ok(city);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("A City with the same name already exists.");
                }

                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult>PutAsync(City city)
        {
            try
            {
                _context.Update(city);
                _context.SaveChanges();
                return Ok(city);    
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("A city with the same name already exists.");
                }

                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult>DeleteAsync(int id)
        {
            var city =await _context.Cities.FirstOrDefaultAsync(x=>x.Id == id);
            if (city == null)
            {
                return NotFound();
            }
            _context.Remove(city);
            _context.SaveChangesAsync();
            return Ok(city);    
        }
    }
}
