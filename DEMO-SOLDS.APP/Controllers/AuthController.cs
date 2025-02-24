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
        
        [HttpPost]
        [Route("CreateRequest")]
        public IActionResult CreateRequest(AuxInvoiceModel obj)
        {
            var user = context.AspNetUsers.FirstOrDefault(x => x.Id == obj.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "Usuario no encontrado." });
            }

            string userEmail = user.Email ?? string.Empty;
            string employeePhone = user.Phone ?? string.Empty;
            string employeePrefix = user.Prefix ?? string.Empty;
            string employeeFullName = user.Name + " " + user.FirstLastName ?? string.Empty;

            // Obtener la zona horaria de Perú
            TimeZoneInfo peruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            DateTime currentTimePeru = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, peruTimeZone);

            // Obtener el último código de factura y generar el siguiente número
            string? latestInvoiceNumber = context.Invoices
                .OrderByDescending(i => i.CreatedOn)
                .Select(i => i.InvoiceCode)
                .FirstOrDefault();

            int nextInvoiceNumber = 1;
            if (!string.IsNullOrEmpty(latestInvoiceNumber) && int.TryParse(new string(latestInvoiceNumber.Where(char.IsDigit).ToArray()), out int lastNumber))
            {
                nextInvoiceNumber = lastNumber + 1;
            }

            Invoices newInvoice = new Invoices
            {
                Id = Guid.NewGuid(),
                InvoiceCode = nextInvoiceNumber.ToString(),
                IdentificationType = obj.IdentificationType,
                DocumentInfo = obj.DocumentInfo,
                IdentificationInfo = obj.IdentificationInfo,
                Telephone = obj.Telephone,
                Email = obj.Email,
                SelectedCategory = obj.SelectedCategory,
                SelectedMeasures = JsonConvert.SerializeObject(obj.SelectedMeasures),
                MeasureQuantities = JsonConvert.SerializeObject(obj.MeasureQuantities),
                DeliveryType = obj.DeliveryType,
                SelectedDistrict = obj.SelectedDistrict,
                Truck20TN = obj.Truck20TN,
                Truck32TN = obj.Truck32TN,
                Truck9TN = obj.Truck9TN,
                ProductsList = JsonConvert.SerializeObject(obj.ProductsList),
                FleteList = JsonConvert.SerializeObject(obj.FleteList),
                TotalWeight = obj.TotalWeight,
                Subtotal = obj.Subtotal,
                IgvRate = obj.IgvRate,
                TotalInvoice = obj.TotalInvoice,
                CreatedBy = userEmail,
                CreatedOn = currentTimePeru,
                LastUpdatedBy = userEmail,
                LastUpdatedOn = currentTimePeru,
                StatusOrder = 0,
                StatusName = "En revisión",
                IsDeleted = false,
                IsParihuelaNeeded = obj.IsParihuelaNeeded,
                CantParihuela = obj.CantParihuela,
                CostParihuela = obj.CostParihuela,
                TotalPriceParihuela = obj.TotalPriceParihuela,
                Address = obj.Address,
                Employee = obj.UserId.ToString(),
                TotalOfPieces = obj.TotalOfPieces,
                UnitPiece = obj.UnitPiece,
                Contact = obj.Contact,
                UserId = obj.UserId,
                Reference = obj.Reference,
                DiscountApplies = obj.DiscountApplies,
                PercentageOfDiscount = obj.PercentageOfDiscount,
                IsOtherDistrict = obj.IsOtherDistrict,
                ManualTotalPriceFlete = obj.ManualTotalPriceFlete
            };

            context.Add(newInvoice);
            context.SaveChanges();

            return Ok(new { EmployeePhone = employeePhone, InvoiceCode = newInvoice.InvoiceCode, EmployeePrefix = employeePrefix, EmployeeFullName = employeeFullName });
        }
        
        [HttpPost]
        [Route("CreateVisit")]
        public IActionResult CreateVisit(VisitModel obj)
        {
            if (obj == null)
            {
                return BadRequest(new { message = "El modelo de visita no puede estar vacío." });
            }

            var user = context.AspNetUsers.FirstOrDefault(x => x.Id.ToString() == obj.CreatedBy);
            if (user == null)
            {
                return BadRequest(new { message = "Ejecutivo no válido." });
            }

            string employeeFullName = $"{user.Name} {user.FirstLastName}".Trim();
            string userEmail = user.Email ?? string.Empty;
            string employeePhone = user.Phone ?? string.Empty;
            string employeePrefix = user.Prefix ?? string.Empty;

            TimeZoneInfo peruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            DateTime currentTimePeru = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, peruTimeZone);

            // Obtener el último código de factura y generar el siguiente número
            string? latestNumber = context.Visit
                .OrderByDescending(i => i.CreatedOn)
                .Select(i => i.VisitCode)
                .FirstOrDefault();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(latestNumber) && int.TryParse(new string(latestNumber.Where(char.IsDigit).ToArray()), out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }

            Visit visit = new Visit
            {
                Id = Guid.NewGuid(),
                Client = obj.Client,
                Work = obj.Work,
                WorkAddress = obj.WorkAddress,
                Contacts = JsonConvert.SerializeObject(obj.Contacts),
                VisitReason = obj.VisitReason == "Otro" ? obj.OtherReason : obj.VisitReason,
                CreatedBy = obj.CreatedBy,
                CreatedOn = currentTimePeru,
                IsDeleted = false,
                VisitCode = nextNumber.ToString(),
                StatusOrder = 1,
                StatusName = "En proceso",
                Comment = ""
            };

            // Guardar la visita en la base de datos
            context.Visit.Add(visit);
            context.SaveChanges();

            // Respuesta con información detallada
            return Ok(new
            {
                message = "Visita creada exitosamente.",
                EmployeePhone = employeePhone,
                VisitCode = visit.VisitCode,
                CreatedBy = employeeFullName,
                CreatedOn = currentTimePeru,
                EmployeePrefix = employeePrefix
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
