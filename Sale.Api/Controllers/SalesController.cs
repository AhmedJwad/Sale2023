using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Shared.DTOs;
using Sale.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Sale.Shared.Enums;

namespace Sale.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IOrdersHelper _ordersHelper;
        private readonly IUserHelper _userHelper;

        public SalesController(DataContext context, IOrdersHelper ordersHelper, IUserHelper userHelper)
        {
            _context = context;
            _ordersHelper = ordersHelper;
            _userHelper = userHelper;
        }
        [HttpGet]
        public async Task<ActionResult>Get([FromQuery]PaginationDTO pagination)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == User.Identity!.Name);
            if(user == null)
            {
                return BadRequest("User not found");
            }
            var querable= _context.Sales.Include(x=>x.User).Include(x=>x.SaleDetails)
                .ThenInclude(x=>x.Product).AsQueryable();
            var isAdmin = await _userHelper.IsUserinRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                querable = querable.Where(x => x.User!.Email == User.Identity!.Name);
            }
            return Ok (await querable.OrderByDescending(x=>x.Date).Paginate(pagination).ToListAsync());
        }
        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPages([FromQuery]PaginationDTO pagination)
        {
            var user=await _context.Users.FirstOrDefaultAsync(x=>x.Email == User.Identity!.Name);   
            if(user == null)
            {
                return BadRequest("User not found");
            }
            var querable =  _context.Sales.AsQueryable();
            var isAdmin = await _userHelper.IsUserinRoleAsync(user, UserType.Admin.ToString());
            if(!isAdmin)
            {
                querable=querable.Where(x=>x.User.Email== User.Identity!.Name);
            }
            double count = await querable.CountAsync();
            double totalPage = Math.Ceiling(count / pagination.REcordNumber);
            return Ok(totalPage);
        }
        [HttpPost]
        public async Task<ActionResult> Post(SaleDTO saleDTO)
        {
            var response = await _ordersHelper.ProcessOrderAsync(User.Identity!.Name!, saleDTO.Remarks);
            if(response.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(response.Message);
        }
    }
}
