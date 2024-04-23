using Azure.Identity;
using DEMO_SOLDS.APP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R2.DEMO.APP.Context;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DEMO_SOLDS.APP.Controllers
{
    [EnableCors("corsRules")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext context;
        public AuthController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            this.context = context;
        }
        [HttpPost("Register")]
        public ActionResult Register(Users newUser)
        {
            try
            {
                var jwt = _configuration.GetSection("Jwt").Get<Jwt>();
                if (context.AspNetUsers.Any(u => u.Email == newUser.Email))
                {
                    return BadRequest("El correo ya se encuentra registrado.");
                }
                string prefix = $"{newUser.Name[0]}{newUser.FirstLastName[0]}{newUser.SecondLastName[0]}".ToUpper();
                string password = newUser.Password;
                string passwordHash = ComputeHmacSha256(password, jwt.Key);
                Users users = new Users
                {
                    Id = Guid.NewGuid(),
                    Name = newUser.Name,
                    Email = newUser.Email,
                    Password = passwordHash,
                    FirstLastName = newUser.FirstLastName,
                    SecondLastName = newUser.SecondLastName,
                    Prefix = prefix,
                    IsApproved = false,
                    IsDeleted = false
                };

                context.AspNetUsers.Add(users);

                context.SaveChanges();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] dynamic optData)
        {
            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();
            var data = JsonConvert.DeserializeObject<dynamic>(optData.ToString());
            string email = data.email;
            string password = data.password;
            string passwordHash = ComputeHmacSha256(password, jwt.Key);
            Users? user = context.AspNetUsers?.FirstOrDefault(x => x.Email == email && x.Password == passwordHash && x.IsDeleted!= true);

            if (user == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Credenciales incorrectas.",
                    token = ""
                });
            }
            else if (user.IsApproved != true)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "El usuario no tiene acceso.",
                    token = ""
                });
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                success = true,
                message = "Credenciales correctas.",
                token = tokenString,
                user = user.Name + " " + user.FirstLastName,
                identificator = user.Id,
            });
        }

        

        private string ComputeHmacSha256(string password, string key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
