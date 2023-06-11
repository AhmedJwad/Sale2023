using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Sale.WEB.Auth
{
    public class AuthenticationProviderTest : AuthenticationStateProvider
    {
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var anonimous = new ClaimsIdentity();
            var zuluUser = new ClaimsIdentity(new List<Claim>
            {
                new Claim("FirstName", "Ahmed"),
                new Claim("LastName", "Almurshadi"),
                new Claim(ClaimTypes.Name, "AhmedAlmurshadi@yopmail.com"),
                new Claim(ClaimTypes.Role, "Admin")
            },
            authenticationType: "test");
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(zuluUser)));
        }
    }
}
