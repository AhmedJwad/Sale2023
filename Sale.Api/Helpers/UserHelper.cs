using Microsoft.AspNetCore.Identity;
using Sale.Api.Data;
using Sale.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Sale.Shared.DTOs;

namespace Sale.Api.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UserHelper(DataContext context,UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager)
        {
           _context = context;
           _userManager = userManager;
           _roleManager = roleManager;
           _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
          return await _userManager.CreateAsync(user, password);
        }
      

        public async Task AddUsertoRoleAsync(User user, string roleName)
        {
         await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
           bool roleExist=await _roleManager.RoleExistsAsync(roleName);
            if(!roleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                   Name = roleName
                });
            }
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
           return await _userManager.ConfirmEmailAsync(user, token);
        }


         public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
           return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<User> GetUserAsync(string email)
        {
            var user = await _context.Users.Include(x => x.City!).ThenInclude(s => s.State!).ThenInclude (u=> u.country!)
                 .FirstOrDefaultAsync(x => x.Email == email);
            return user!;
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            var user = await _context.Users.Include(x => x.City).ThenInclude(x => x.State)
                 .ThenInclude(x => x.country).FirstOrDefaultAsync(x => x.Id == userId.ToString());
            return user;
        }

        public async Task<bool> IsUserinRoleAsync(User user, string roleName)
        {
           return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginDTO model)
        {
            return await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }
}
