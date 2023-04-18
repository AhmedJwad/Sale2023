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
            _context.countries.Add(new Country
            {
                Name = "Iraq",
                States = new List<State>()
                {
                    new State { Name="Bablyon" ,
                    Cities=new List<City>(){
                    new City{Name="city center"},
                     new City{Name="Hilla"},
                     new City{Name="hey alhussein"},
                    }
                    }
            }
            });
            _context.countries.Add(
                new Country { Name = "Oman" ,
                States=new List<State>(){
                new State {Name="state oman",
                Cities=new List<City>(){ new City { Name="city center oman"} } } }
                
                });
            }
         await _context.SaveChangesAsync();
        }
    }
}
