using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Shared.Entities;
using Sale.Shared.Enums;
using Sale.Shared.Response;


namespace Sale.Api.Helpers
{
    public class OrdersHelper : IOrdersHelper
    {
        private readonly DataContext _context;

        public OrdersHelper(DataContext context)
        {
            _context = context;
        }
        public async Task<Response> ProcessOrderAsync(string email, string remarks)
        {
            var user=await _context.Users.FirstOrDefaultAsync(x=>x.Email == email);
            if (user == null)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "user not found",
                };
            }

                var temporalSales = await _context.TemporalSales.Include(x => x.Product)
                    .Where(x => x.User!.Email == email).ToListAsync();
              Response response =await CheckInventoryAsync(temporalSales);
                if(!response.IsSuccess)
                {
                    return response;
                }
                Order sale = new()
                {
                    Date = DateTime.UtcNow,
                    User = user,
                    Remarks = remarks,
                    SaleDetails = new List<SaleDetail>(),
                    OrderStatus=OrderStatus.New,
                };
                foreach (var item in temporalSales)
                {
                    sale.SaleDetails.Add(new SaleDetail
                    {
                        Product=item.Product,
                        Quantity=item.Quantity,
                        Remarks=item.Remarks,
                    });
                    Product? product = await _context.Products.FindAsync(item.Product!.Id);
                    if(product !=null)
                    {
                        product.Stock = item.Quantity;
                        _context.Products.Update(product);
                    }
                    _context.TemporalSales.Remove(item);


                }
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            return response;       
       
        }

        private async Task<Shared.Response.Response> CheckInventoryAsync(List<TemporalSale> temporalSales)
        {
            Shared.Response.Response response = new Shared.Response.Response { IsSuccess = true };
            foreach (var item in temporalSales)
            {
                Product? product=await _context.Products.FirstOrDefaultAsync(x=>x.Id == item.Product!.Id);
                if(product==null)
                {
                    response.IsSuccess = false;
                    response.Message= $"The product {item.Product!.Name}, is no longer available";
                    return response;
                }
                if(product.Stock <item.Quantity)
                {
                    response.IsSuccess = false;
                    response.Message= $"Sorry we do not have enough stock of the product {item.Product!.Name}," +
                        $" to take your order. Please reduce the amount or replace it with another.";
                    return response;
                }
               
            }
            return response;
        }
    }
}

