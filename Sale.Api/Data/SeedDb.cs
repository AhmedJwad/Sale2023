using Sale.Shared.Entities;

namespace Sale.Api.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }
        public async Task SeedAsync() 
        {
            await _context.Database.EnsureCreatedAsync();
            await CeckcountriesAsync();
        }

        private async Task CeckcountriesAsync()
        {
         if(!_context.countries.Any()) 
            { 
            _context.countries.Add(new Country { Name = "Iraq" });
            _context.countries.Add(new Country { Name = "Oman" });
            }
         await _context.Database.EnsureCreatedAsync();
        }
    }
}
