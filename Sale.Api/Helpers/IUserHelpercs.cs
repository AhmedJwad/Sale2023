using Microsoft.AspNetCore.Identity;
using Sale.Shared.Entities;

namespace Sale.Api.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserAsync(string email);
        Task<IdentityResult> AddUserAsync(User user, string password);
        Task CheckRoleAsync(string roleName);
        Task AddUsertoRoleAsync(User user, string roleName);
        Task<bool>IsUserinRoleAsync(User user, string roleName);   
    }
}
