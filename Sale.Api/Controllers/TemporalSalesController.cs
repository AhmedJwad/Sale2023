using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Shared.DTOs;
using Sale.Shared.Entities;


namespace Sale.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class TemporalSalesController : ControllerBase
    {
        private readonly DataContext _context;

        public TemporalSalesController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(TemporalSaleDTO temporalSaleDTO)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == temporalSaleDTO.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == (User.Identity!.Name));
            if (user == null)
            {
                return NotFound();
            }
            TemporalSale temporalSale = new TemporalSale {
                Product = product,
                User = user,
                Quantity = temporalSaleDTO.Quantity,
                Remarks = temporalSaleDTO.Remarks,
            };
            try
            {
                _context.Add(temporalSale);
                await _context.SaveChangesAsync();
                return Ok(temporalSale);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.TemporalSales.Include(x => x.User!)
                .Include(x => x.Product!).ThenInclude(x => x.productCategories!)
                .ThenInclude(x => x.Category)
                .Include(x => x.Product!).ThenInclude(x => x.productImages)
                .Where(x => x.User!.Email == User.Identity!.Name).ToListAsync());
        }
        [HttpGet("count")]
        public async Task<ActionResult> GetCount()
        {
            return Ok(await _context.TemporalSales.Where(x => x.User!.Email == User.Identity!.Name)
                .SumAsync(x => x.Quantity));
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            return Ok(await _context.TemporalSales.Include(x => x.User!)
                .Include(x => x.Product!).ThenInclude(x => x.productCategories!)
                .ThenInclude(x => x.Category).Include(x => x.Product!).ThenInclude(x => x.productImages)
                .FirstOrDefaultAsync(x => x.Id == id));
        }
        [HttpPut]
        public async Task<ActionResult> Put(TemporalSaleDTO temporalSaleDTO)
        {
            var currentTemporalSale = await _context.TemporalSales.FirstOrDefaultAsync(x => x.Id
            == temporalSaleDTO.Id);
            if (currentTemporalSale == null) { return NotFound(); }
            currentTemporalSale.Remarks = temporalSaleDTO.Remarks;
            currentTemporalSale.Quantity = temporalSaleDTO.Quantity;
            _context.Update(currentTemporalSale);
            await _context.SaveChangesAsync();
            return Ok(temporalSaleDTO);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var temporalSale = await _context.TemporalSales.FirstOrDefaultAsync(x => x.Id == id);
            if (temporalSale == null)
            {
                return NotFound();
            }

            _context.Remove(temporalSale);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
