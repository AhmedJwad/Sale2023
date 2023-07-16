﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            if (user == null)
            {
                return BadRequest("User not valid.");
            }

            var queryable = _context.Sales
                .Include(s => s.User!)
                .Include(s => s.SaleDetails!)
                .ThenInclude(sd => sd.Product)
                .AsQueryable();

            var isAdmin = await _userHelper.IsUserinRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                queryable = queryable.Where(s => s.User!.Email == User.Identity!.Name);
            }

            return Ok(await queryable
                .OrderByDescending(x => x.Date)
                .Paginate(pagination)
                .ToListAsync());
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


        [HttpGet("{id:int}")]
        public async Task<ActionResult>Get(int id)
        {
            var sale = await _context.Sales.Include(x => x.User!)
                .ThenInclude(x => x.City!).ThenInclude(x => x.State!)
                .ThenInclude(x => x.country).Include(x => x.SaleDetails!)
                .ThenInclude(x => x.Product).ThenInclude(x => x.productImages)
                .FirstOrDefaultAsync(x => x.Id == id);
            if(sale ==null)
            {
                return NotFound();
            }
            return Ok(sale);
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

        [HttpPut]
        public async Task<ActionResult> Put(SaleDTO saleDTO)
        {
            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user ==null)
            {
                return NotFound();
            }
            var isAdmin = await _userHelper.IsUserinRoleAsync(user, UserType.Admin.ToString());
            if(!isAdmin && saleDTO.OrderStatus != OrderStatus.Cancelled)
            {
                return BadRequest("Only allowed for administrators");
            }
            var sale = await _context.Sales.Include(s => s.SaleDetails).FirstOrDefaultAsync(x => x.Id == saleDTO.Id);
            if(sale ==null) { return NotFound(); }
            if(saleDTO.OrderStatus==OrderStatus.Cancelled)
            {
                await ReturnStockAsync(sale);

            }
           
            sale.OrderStatus = saleDTO.OrderStatus;
            _context.Update(sale);
            await _context.SaveChangesAsync();
            return Ok(sale);
        }

        private async Task ReturnStockAsync(Order sale)
        {
            foreach (var item in sale.SaleDetails!)
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                if(product !=null)
                {
                    product.Stock += item.Quantity;
                }
              await  _context.SaveChangesAsync();
            }
        }
        
    }
}
