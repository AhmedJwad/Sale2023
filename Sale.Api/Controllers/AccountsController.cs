using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sale.Api.Helpers;
using Sale.Shared.DTOs;
using Sale.Shared.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sale.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly IFileStorage _fileStorage;
        private readonly string _container;

        public AccountsController(IUserHelper userHelper, IConfiguration configuration , IFileStorage fileStorage)
        {
           _userHelper = userHelper;
           _configuration = configuration;
           _fileStorage = fileStorage;
            _container = "users";
        }
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody]UserDTO model)
        {
            User user = model;
            if(!string.IsNullOrEmpty(model.Photo))
            {
                var photoUser = Convert.FromBase64String(model.Photo);
                model.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", _container);

            }
            var result=await _userHelper.AddUserAsync(user, model.Password);  
            if(result.Succeeded)
            {
                await _userHelper.AddUsertoRoleAsync(user, user.UserType.ToString());
                return Ok(BuildToken(user));    
            }

            return BadRequest(result.Errors.FirstOrDefault());
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginDTO model)
        {
            var result = await _userHelper.LoginAsync(model);
            if(result.Succeeded)
            {
                var user = await _userHelper.GetUserAsync(model.Email);
                return Ok(BuildToken(user));
            }
            return BadRequest("email or password incorrect");
        }

        private TokenDTO BuildToken(User user)
        {
            var claims = new List<Claim>
            { 
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                new Claim("Document", user.Document),
                new Claim("FirstName", user.Firstname),
                new Claim("LastName", user.LastName),
                new Claim("Address", user.Address),
                new Claim("Photo", user.Photo ?? string.Empty),
                new Claim("CityId", user.CityId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
            var credential=new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration=DateTime.UtcNow.AddDays(30);
            var token = new JwtSecurityToken(
                issuer:null,
                audience:null,
                claims:claims,
                expires:expiration,
                signingCredentials: credential

                );
            return new TokenDTO
            {
                Token=new JwtSecurityTokenHandler().WriteToken(token),
                Expiration=expiration,
            };
        }
    }
}
