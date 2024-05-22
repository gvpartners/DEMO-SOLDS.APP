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
    public class TrackController : ControllerBase
    {
        private UserService _userService;
        private TrackService _trackService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext context;

        public TrackController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            this.context = context;
            _userService = new UserService(configuration, context);
            _trackService = new TrackService(configuration, context);
        }
        [HttpPost("GenerateExcel")]
        public IActionResult GenerateExcel(ExcelFilters obj)
        {
            try
            {
                var userIdToPrefixMap = context.AspNetUsers
                .Where(u => u.IsDeleted != true)
                .ToDictionary(u => u.Id, u => new { Prefix = u.Prefix, Name = u.Name, FirstLastName = u.FirstLastName });
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("REGISTRO DE PEDIDOS");
                    worksheet.ShowGridLines = false;

                    var data = _trackService.GetFilterDataForExcel(obj);

                    string[] headers = {
                        "CÓDIGO",
                        "CLIENTE",
                        "RUC/DNI",
                        "CATEGORIA",
                        "CANTIDAD",
                        "U.M",
                        "FECHA DE PEDIDO",
                        "FECHA DE ENTREGA",
                        "TIPO DE ENTREGA",
                        "CONTACTO",
                        "TELEFONO",
                        "EJECUTIVO",
                        "ESTADO",
                    };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(2, i + 2).Value = headers[i];
                        worksheet.Cell(2, i + 2).Style.Fill.BackgroundColor = XLColor.Orange;
                        worksheet.Cell(2, i + 2).Style.Font.Bold = true;
                        worksheet.Cell(2, i + 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(2, i + 2).Style.Border.OutsideBorderColor = XLColor.Black;
                    }

                    worksheet.Range("B2:N2").SetAutoFilter();

                    int row = 3;
                    foreach (var track in data)
                    {
                        worksheet.Cell(row, 2).Value = "PD-" + userIdToPrefixMap[track.UserId].Prefix + track?.TrackCode?.PadLeft(6, '0');
                        worksheet.Cell(row, 3).Value = track?.IdentificationInfo;
                        worksheet.Cell(row, 4).Value = track?.DocumentInfo;
                        worksheet.Cell(row, 5).Value = track?.SelectedCategory;
                        worksheet.Cell(row, 6).Value = track?.TotalOfPieces;
                        worksheet.Cell(row, 7).Value = track?.UnitPiece;
                        // Formatear la celda como moneda en soles
                        worksheet.Cell(row, 8).Value = track?.CreatedOn?.ToString("yyyy-MM-dd HH:mm:ss");
                        worksheet.Cell(row, 9).Value = track?.DeliveryDate.ToString("yyyy-MM-dd");
                        worksheet.Cell(row, 10).Value = track?.DeliveryType;
                        worksheet.Cell(row, 11).Value = track?.Contact;
                        worksheet.Cell(row, 12).Value = track?.Telephone;
                        worksheet.Cell(row, 13).Value = userIdToPrefixMap[track.UserId].Name + " " + userIdToPrefixMap[track.UserId].FirstLastName;
                        worksheet.Cell(row, 14).Value = track?.StatusName;

                        for (int i = 2; i <= 14; i++)
                        {
                            worksheet.Cell(row, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(row, i).Style.Border.OutsideBorderColor = XLColor.Black;
                            worksheet.Cell(row, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            worksheet.Column(i).AdjustToContents();
                        }

                        row++;

                    }

                    using (var originalStream = new MemoryStream())
                    {
                        workbook.SaveAs(originalStream);
                        var content = new MemoryStream(originalStream.ToArray());

                        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        var fileName = "track.xlsx";

                        return File(content, contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpPost("GetAllTracks")]
        public async Task<ActionResult<dynamic>> GetAllTracks(TrackPage pag)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _trackService.GetAllTracks(pag);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpGet("GetTrackById/{Id}")]
        public async Task<ActionResult<dynamic>> GetTrackById(Guid Id)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _trackService.GetTrackById(Id);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }
        [HttpPost("CreateTrack")]
        public async Task<ActionResult<dynamic>> CreateTrack(TrackModel obj)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _trackService.CreateTrack(obj);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPost("UpdateTrack/{Id}")]
        public async Task<ActionResult<dynamic>> UpdateTrack(Guid Id, TrackModel obj)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _trackService.UpdateTrack(Id,obj);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }
        [HttpPost("RemoveTrack/{Id}")]
        public async Task<ActionResult<dynamic>> RemoveTrack(Guid Id)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _trackService.RemoveTrack(Id);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPost("UpdateStatusTrack/{Id}/{OrderStatus}")]
        public async Task<ActionResult<dynamic>> UpdateStatusTrack(Guid Id, int OrderStatus)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _trackService.UpdateStatusTrack(Id, OrderStatus);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPost("DuplicateTrack/{id}/{userId}")]
        public async Task<IActionResult> DuplicateTrack(Guid id,Guid userId)
        {
            try
            {
                await Task.Run(() =>
                {
                    _trackService.DuplicateTrack(id,userId);
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
                    response = _trackService.GetCommentById(id);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }
        [HttpPost("UpdateCommentbyId/{id}/{newComment}")]
        public async Task<IActionResult> UpdateCommentbyId(Guid id, string newComment)
        {
            try
            {
                await Task.Run(() =>
                {
                    _trackService.UpdateCommentbyId(id,newComment);
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
