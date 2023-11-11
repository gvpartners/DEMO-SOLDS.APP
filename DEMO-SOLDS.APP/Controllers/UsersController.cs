using R2.DEMO.APP.Context;
using DEMO_SOLDS.APP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using DEMO_SOLDS.APP.Services;

namespace R2.DEMO.APP.Controllers
{
    [EnableCors("corsRules")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private UserService _userService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext context;

        public UsersController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            this.context = context;
            _userService = new UserService(configuration, context);
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<dynamic>> GetAllUsers()
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _userService.GetAllUsers();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPost("EditUser")]
        public async Task<ActionResult<dynamic>> EditUser(Users newUser)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _userService.EditUser(newUser);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPut("RemoveItem/{id}")]
        public async Task<IActionResult> RemoveItem(Guid id)
        {
            try
            {
                await Task.Run(() =>
                {
                    _userService.RemoveItem(id);
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<ActionResult<dynamic>> GetUserById(Guid Id)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _userService.GetUserById(Id);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }
        [HttpPut("UpdateUserById")]
        public async Task<IActionResult> UpdateUserById(Users updatedUser)
        {
            try
            {
                await Task.Run(() =>
                {
                    _userService.UpdateUserById(updatedUser);
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("UpdatePasswordById/{id}/{password}")]
        public async Task<IActionResult> UpdatePasswordById(Guid id,string password)
        {
            try
            {
                await Task.Run(() =>
                {
                    _userService.UpdatePasswordById(id, password);
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
