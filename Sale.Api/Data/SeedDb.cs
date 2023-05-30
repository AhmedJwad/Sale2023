using Microsoft.EntityFrameworkCore;
using Sale.Api.Helpers;
using Sale.Api.Services;
using Sale.Shared.Entities;
using Sale.Shared.Enums;
using Sale.Shared.Response;

namespace Sale.Api.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IApiService _apiService;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context , IApiService apiService, IUserHelper userHelper)
        {
            _context = context;
           _apiService = apiService;
           _userHelper = userHelper;
        }
        public async Task SeedAsync() 
        {
            await _context.Database.EnsureCreatedAsync();
            //await CeckcountriesAsync();
            await CheckRoleAsync();
            await CheckUserAsync("1010", "Ahmed", "murshadi", "AhmedAlmurshadi@yopmail.com", "07504412021", "iraq-babylon"
                , UserType.Admin);
            
        }

        private async Task CheckRoleAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<User> CheckUserAsync(string document, string firstname, string lastname, string email, string phonenumber,
            string Adress, UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);
            var city=await _context.Cities.FirstOrDefaultAsync(x=>x.Name=="babil");
            if(city==null)
            {
                city = await _context.Cities.FirstOrDefaultAsync();
            }
            if (user == null) 
            {
                user = new User
                {
                    Firstname=firstname,
                    LastName=lastname,
                    Document=document,
                    Email=email,
                    PhoneNumber=phonenumber,
                    Address=Adress,
                    City=city,
                    UserType=userType,
                    CityId=city.Id,
                };
                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUsertoRoleAsync(user, userType.ToString());

            }
            return user;
        }

        private async Task CeckcountriesAsync()
        {
            {
                Response responseCountries = await _apiService.GetListAsync<CountryResponse>("/v1", "/countries");
                if (responseCountries.IsSuccess)
                {
                    List<CountryResponse> countries = (List<CountryResponse>)responseCountries.Result!;
                    foreach (CountryResponse countryResponse in countries)
                    {
                        Country country = await _context.countries!.FirstOrDefaultAsync(c => c.Name == countryResponse.Name!)!;
                        if (country == null)
                        {
                            country = new() { Name = countryResponse.Name!, States = new List<State>() };
                            Response responseStates = await _apiService.GetListAsync<StateResponse>("/v1", $"/countries/{countryResponse.Iso2}/states");
                            if (responseStates.IsSuccess)
                            {
                                List<StateResponse> states = (List<StateResponse>)responseStates.Result!;
                                foreach (StateResponse stateResponse in states!)
                                {
                                    State state = country.States!.FirstOrDefault(s => s.Name == stateResponse.Name!)!;
                                    if (state == null)
                                    {
                                        state = new() { Name = stateResponse.Name!, Cities = new List<City>() };
                                        Response responseCities = await _apiService.GetListAsync<CityResponse>("/v1", $"/countries/{countryResponse.Iso2}/states/{stateResponse.Iso2}/cities");
                                        if (responseCities.IsSuccess)
                                        {
                                            List<CityResponse> cities = (List<CityResponse>)responseCities.Result!;
                                            foreach (CityResponse cityResponse in cities)
                                            {
                                                if (cityResponse.Name == "Mosfellsbær" || cityResponse.Name == "Șăulița")
                                                {
                                                    continue;
                                                }
                                                City city = state.Cities!.FirstOrDefault(c => c.Name == cityResponse.Name!)!;
                                                if (city == null)
                                                {
                                                    state.Cities.Add(new City() { Name = cityResponse.Name! });
                                                }
                                            }
                                        }
                                        if (state.CityNumber > 0)
                                        {
                                            country.States.Add(state);
                                        }
                                    }
                                }
                            }
                            if (country.StateNumber > 0)
                            {
                                _context.countries.Add(country);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
        }

    }
}
