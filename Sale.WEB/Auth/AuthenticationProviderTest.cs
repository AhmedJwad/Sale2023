using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Sale.WEB.Auth
{
    public class AuthenticationProviderTest : AuthenticationStateProvider

    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            await Task.Delay (3000);
            var anonimous = new ClaimsIdentity();
            var Ahmeduser = new ClaimsIdentity(new List<Claim>
            {
                new Claim("FirstName", "Ahmed"),
                new Claim("LastName", "Almurshadi"),
                 new Claim(ClaimTypes.Name, "Ahmed@yopmail.com"),
                 new Claim(ClaimTypes.Role, "Admin"),

            }, authenticationType:"test");
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(Ahmeduser)));   
        }
    }
}
