using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sale.Api.Helpers;
using Sale.Shared.DTOs;
using Sale.Shared.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
        private readonly IMailHelper _mailHelper;
        private readonly string _container;

        public AccountsController(IUserHelper userHelper, IConfiguration configuration ,
            IFileStorage fileStorage, IMailHelper mailHelper)
        {
           _userHelper = userHelper;
           _configuration = configuration;
           _fileStorage = fileStorage;
            _mailHelper = mailHelper;
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
                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail", "accounts", new
                {
                    userid = user.Id,
                    token = myToken
                }, HttpContext.Request.Scheme, _configuration["UrlWEB"]);

                var response = _mailHelper.SendMail(user.Fullname, user.Email!,
                    $"Sales - Account Confirmation",
                    $"<h1>Sales - Account Confirmation</h1>" +
                    $"<p>To enable the user, please click 'Confirm Email':</p>" +
                    $"<b><a href ={tokenLink}>Confirm Email</a></b>");

                if (response.IsSuccess)
                {
                    return NoContent();
                }

                return BadRequest(response.Message);
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
            if (result.IsLockedOut)
            {
                return BadRequest("You have exceeded the maximum number of attempts, your account is locked, please try again in 5 minutes.");
            }

            if (result.IsNotAllowed)
            {
                return BadRequest("The user has not been enabled, you must follow the instructions in the email sent to enable the user.");
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

        [HttpPut]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult>Put(User user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.Photo))
                {
                    var photoUser=Convert.FromBase64String(user.Photo);
                    user.Photo = await _fileStorage.SaveFileAsync(photoUser, ".jpg", _container);
                }
                var currentUser = await _userHelper.GetUserAsync(user.Email!);
                if (currentUser == null) { return NotFound(); }

                currentUser.Firstname = user.Firstname;
                currentUser.Document = user.Document;
                currentUser.LastName = user.LastName;
                currentUser.Address = user.Address;
                currentUser.PhoneNumber = user.PhoneNumber;
                currentUser.Photo=!string.IsNullOrEmpty(user.Photo)&& user.Photo!=currentUser.Photo
                    ?user.Photo:currentUser.Photo;
                currentUser.CityId = user.CityId;   
                var result=await _userHelper.UpdateUserAsync(currentUser);
                if (result.Succeeded)
                {
                    return Ok(BuildToken(currentUser));
                }
                return BadRequest(result.Errors.FirstOrDefault());
            }
            catch (Exception EX)
            {

                return BadRequest(EX.Message);
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult>Get()
        {
            return Ok(await _userHelper.GetUserAsync(User.Identity!.Name!));
        }
        [HttpPost("changePassword")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> changePassword(ChangePasswordDTO model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }
            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if(user==null)
            {
                return NotFound();
            }
            var result = await _userHelper.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors!.FirstOrDefault()!.Description);
            }
            return NoContent();
        }
        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            token = token.Replace(" ", "+");
            var user =await _userHelper.GetUserAsync(new Guid(userId));
            if(user==null)
            {
                return NotFound();
            }
            var result= await _userHelper.ConfirmEmailAsync(user, token);
            if(!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()!.Description);
            }
            return NoContent(); 
        }
        [HttpPost("ResedToken")]
        public async Task<ActionResult> ResedToken([FromBody] EmailDTO model)
        {
            User user = await _userHelper.GetUserAsync(model.Email);
            if(user==null) { return NotFound();}

            var mytoken=await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            var tokenlink=Url.Action("ConfirmEmail", "Accounts", new
            {
                userid=user.Id,
                token=mytoken,

            },HttpContext.Request.Scheme,_configuration["UrlWEB"]);
            var response = _mailHelper.SendMail(user.Fullname, user.Email!,
                $"Sales - Account Confirmation",
                $"<h1> Sales - Account Confirmation</h1>"+
                $"<p> To enable the user, please click 'Confirm Email':</p>"+
                $"<p><a href ={tokenlink}>Confirm Email</a></p>");
            if(response.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(response.Message);
        }
        [HttpPost("RecoverPassword")]
        public async Task<ActionResult> RecoverPassword([FromBody] EmailDTO model)
        {
            User user = await _userHelper.GetUserAsync(model.Email);
            if(user==null) { return NotFound();}
            var mytoken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            var tokenlink = Url.Action("ResetPassword", "accounts", new
            {
                userid = user.Id,
                token = mytoken,


            }, HttpContext.Request.Scheme, _configuration["UrlWEB"]);
            var response = _mailHelper.SendMail(user.Fullname, user.Email!,
               $"Sales - Password Recovery",
               $"<h1>Sales - Password Recovery</h1>" +
               $"<p>To recover your password, please click 'Recover Password':</p>" +
               $"<b><a href={tokenlink}>Recover password</a></b>");
            if(response.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(response.Message);

        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            User user = await _userHelper.GetUserAsync(model.Email);
            if (user == null) { return NotFound();}
            var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
            if(result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors!.FirstOrDefault()!.Description);
        }
    }
}
