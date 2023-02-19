using Microsoft.EntityFrameworkCore;
using Sale.Shared.Entities;

namespace Sale.Api.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext>options):base(options) 
        {
            
        }

        public DbSet<Country>
    }
}
