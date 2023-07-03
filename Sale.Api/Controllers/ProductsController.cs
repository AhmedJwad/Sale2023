﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Shared.DTOs;
using Sale.Shared.Entities;




namespace Sale.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;

        public ProductsController(DataContext context , IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _context.Products.Include(x => x.productCategories).Include(x => x.productImages)
            .AsQueryable();
            if(!string.IsNullOrWhiteSpace(paginationDTO.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(paginationDTO.Filter.ToLower()));
            }
            return Ok(await queryable.OrderBy(x=>x.Name).Paginate(paginationDTO).ToListAsync());
        }
        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable=_context.Products.Include(x=>x.productCategories)
                .Include(x=>x.productImages).AsQueryable();
            if(!string.IsNullOrWhiteSpace(paginationDTO.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(paginationDTO.Filter.ToLower()));
            }
            double count=await queryable.CountAsync();
            double totlapage = Math.Ceiling(count / paginationDTO.REcordNumber);
            return Ok(totlapage);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            var product = await _context.Products.Include(x => x.productCategories!)
                .ThenInclude(x => x.Category).Include(x => x.productImages)
                .FirstOrDefaultAsync(x=>x.Id==id);
            if(product ==null)
            {
                return NotFound();
            }
            return Ok(product);
             
        }
        [HttpPost]
        public async Task<ActionResult>Post(ProductDTO productDTO)
        {
            try
            {
                Product newproduct = new()
                {
                    Name=productDTO.Name,
                    Description=productDTO.Description,
                    Price=productDTO.Price,
                    Stock=productDTO.Stock,
                    productCategories=new List<ProductCategory>(),
                    productImages=new List<ProductImage>()
                };
                foreach (var productimage in productDTO.productImages!)
                {
                    var photoproduct = Convert.FromBase64String(productimage);
                    newproduct.productImages.Add(new ProductImage
                    {
                        Image = await _fileStorage.SaveFileAsync(photoproduct, ".jpg", "products")
                    });
                }
                foreach (var categrory in productDTO.ProductCategoriesIds!)
                {
                    newproduct.productCategories.Add(new ProductCategory
                    {
                        Category=await _context.Categories.FirstOrDefaultAsync(x=>x.Id
                        ==categrory)
                    });
                }
                _context.Add(newproduct);
                await _context.SaveChangesAsync();
                return Ok(productDTO);
            }
            catch (DbUpdateException dbUpdateException)
            {

               if(dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("There is already a product with the same name.");
                }
                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }

        }
        [HttpPut]
        public async Task<ActionResult> PutAsync(ProductDTO productDTO)
        {
            try
            {
                var product = await _context.Products
                    .Include(x => x.productCategories)
                    .FirstOrDefaultAsync(x => x.Id == productDTO.Id);
                if (product == null)
                {
                    return NotFound();
                }

                product.Name = productDTO.Name;
                product.Description = productDTO.Description;
                product.Price = productDTO.Price;
                product.Stock = productDTO.Stock;
                product.productCategories = productDTO.ProductCategoriesIds!.Select(x => new ProductCategory { CategoryId = x }).ToList();

                _context.Update(product);
                await _context.SaveChangesAsync();
                return Ok(productDTO);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("There is already a city with the same name.");
                }

                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }


        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult>DeleteAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) { return NotFound(); }
            _context.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}