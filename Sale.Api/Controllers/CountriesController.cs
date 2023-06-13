using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Shared.DTOs;
using Sale.Shared.Entities;
using System.Linq;

namespace Sale.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly DataContext _context;

        public CountriesController(DataContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        [HttpGet("combo")]
        public async Task<ActionResult>GetCombo()
        {
            return Ok(await _context.countries.ToListAsync());
        }

       
        
        [HttpGet]
        public async Task<ActionResult> GetAsync([FromQuery]PaginationDTO pagination)
        {
            var queryable = _context.countries.Include(x => x.States).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return Ok(await queryable.OrderBy(x=>x.Name).Paginate(pagination).ToListAsync());
        }
        [HttpGet("totalPages")]
        public async Task<IActionResult> GetPages([FromQuery] PaginationDTO pagination)
        {
            var queryabl= _context.countries.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryabl = queryabl.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count =await queryabl.CountAsync();
            double totalPage = Math.Ceiling(count / pagination.REcordNumber);
            return Ok(totalPage);
        }

      

        [HttpGet("Full")]
        public async Task<IActionResult>GetFullAsync()
        {
            return Ok(await _context.countries.Include(x=>x.States).ThenInclude(c=>c.Cities).ToListAsync());  
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync(int id )
        {
          var country=  await _context.countries.Include(x=>x.States).ThenInclude(x=>x.Cities).FirstOrDefaultAsync(x=>x.Id==id);
            if(country==null)
            {
                return NotFound();
            }
            return Ok(country);
        }

        [HttpPost]
        public async Task<IActionResult>PostAsync(Country country)
        {
            _context.Add(country);
            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("There is already a country with the same name.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }


        }
        [HttpPut]
        public async Task<IActionResult> PutAsync(Country country)
        {
            _context.Update(country);
            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("There is already a country with the same name.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }


        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var country = await _context.countries.FirstOrDefaultAsync(x => x.Id == id);
            if (country == null)
            {
                return NotFound();
            }
            _context.Remove(country);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
