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
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;

namespace DEMO_SOLDS.APP.Controllers
{
    [EnableCors("corsRules")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VisitController : ControllerBase
    {
        private UserService _userService;
        private VisitService _visitService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext context;

        public VisitController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            this.context = context;
            _userService = new UserService(configuration, context);
            _visitService = new VisitService(configuration, context);
        }
        
        [HttpPost("GetAllVisits")]
        public async Task<ActionResult<dynamic>> GetAllVisits(VisitPage pag)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _visitService.GetAllVisits(pag);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPost("UpdateVisitStatus")]
        public async Task<ActionResult<dynamic>> UpdateVisitStatus(Guid visitId, int status)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _visitService.UpdateVisitStatus(visitId, status);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }
        [HttpPost("UpdateCommentbyId")]
        public async Task<IActionResult> UpdateCommentbyId(Guid visitId, string newComment)
        {
            try
            {
                await Task.Run(() =>
                {
                    _visitService.UpdateCommentbyId(visitId, newComment);
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetCommentById/{id}")]
        public async Task<ActionResult<dynamic>> GetCommentById(Guid id)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _visitService.GetCommentById(id);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }
        [HttpPost("RemoveVisit/{Id}")]
        public async Task<ActionResult<dynamic>> RemoveVisit(Guid Id)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _visitService.RemoveVisit(Id);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }
        [HttpPost("UpdateVisit")]
        public async Task<ActionResult<dynamic>> UpdateVisit(Guid visitId, [FromBody] Visit updatedVisit)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _visitService.UpdateVisit(visitId, updatedVisit);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }
    }
}
