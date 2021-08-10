using ChallengeDisney.Entities;
using ChallengeDisney.Services;
using ChallengeDisney.ViewModels.Auth.Login;
using ChallengeDisney.ViewModels.Auth.Register;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeDisney.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;

        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, IMailService mailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mailService = mailService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);

            if(userExists != null)
            {
                return BadRequest();
            }

            var user = new User
            {
                Email = model.Email,
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {                        
                        Message = $"User Creation Failed! Error: {string.Join(',', result.Errors.Select(x => x.Description))}"
                    });

            }
            
            await _mailService.SendEmailAsync(user);

            return StatusCode(StatusCodes.Status201Created, new
            {               
                Message = "Congratulations, your account has been created!"
            });           
        }        

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return BuildToken(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return BadRequest(ModelState);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        private IActionResult BuildToken(LoginRequestModel model)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, model.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Secret_Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: "https://localhost:5001",
               audience: "https://localhost:5001",
               expires: DateTime.Now.AddHours(3),
               claims: claims,
               signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)                
            });

        }
    }
}

    
