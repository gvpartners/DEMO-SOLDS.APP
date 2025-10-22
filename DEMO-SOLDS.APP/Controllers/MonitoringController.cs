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
    public class MonitoringController : ControllerBase
    {
        private UserService _userService;
        private MonitoringService _monitoringService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext context;

        public MonitoringController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            this.context = context;
            _userService = new UserService(configuration, context);
            _monitoringService = new MonitoringService(configuration, context);
        }

        [HttpPost("GenerateExcel")]
        public IActionResult GenerateExcel(ExcelFilters obj)
        {
            try
            {
                // Obtener la zona horaria de Perú
                TimeZoneInfo peruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                // Obtener la hora actual en la zona horaria de Perú
                DateTime currentTimePeru = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, peruTimeZone);

                var userIdToPrefixMap = context.AspNetUsers
                .Where(u => u.IsDeleted != true)
                .ToDictionary(u => u.Id, u => new { Prefix = u.Prefix, Name = u.Name, FirstLastName = u.FirstLastName });
                
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("REGISTRO DE SEGUIMIENTOS");
                    worksheet.ShowGridLines = false;

                    var data = _monitoringService.GetFilterDataForExcel(obj);

                    string[] headers = { 
                        "FECHA",
                        "# SEG",
                        "CLIENTE",
                        "RUC/DNI",
                        "TIPO DE ENTREGA",
                        "DISTRITO",
                        "CATEGORIA",
                        "SEGMENTO",
                        "RESPONSABLE",
                        "EJECUTIVO",
                        "ESTADO",
                        "CANTIDAD",
                        "PLAZO ASIGNADO (Días)",
                        "FECHA REQUERIMIENTO",
                        "FECHA COTIZADA",
                        "TIEMPO REAL (Días)",
                        "CONTACTO",
                        "TELEFONO"
                    };
                    
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(2, i + 2).Value = headers[i];
                        worksheet.Cell(2, i + 2).Style.Fill.BackgroundColor = XLColor.Orange;
                        worksheet.Cell(2, i + 2).Style.Font.Bold = true;
                        worksheet.Cell(2, i + 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(2, i + 2).Style.Border.OutsideBorderColor = XLColor.Black;
                    }

                    worksheet.Range("B2:S2").SetAutoFilter();

                    int row = 3;
                    foreach (var monitoring in data)
                    {
                        worksheet.Cell(row, 2).Value = monitoring.RequirementDate?.ToString("yyyy-MM-dd") ?? "";
                        worksheet.Cell(row, 3).Value = monitoring.MonitoringCode;
                        worksheet.Cell(row, 4).Value = monitoring.DocumentInfo;
                        worksheet.Cell(row, 5).Value = monitoring.IdentificationInfo;
                        worksheet.Cell(row, 6).Value = monitoring.DeliveryType ?? "";
                        worksheet.Cell(row, 7).Value = monitoring.SelectedDistrict;
                        worksheet.Cell(row, 8).Value = monitoring.SelectedCategory;
                        worksheet.Cell(row, 9).Value = monitoring.Segment;
                        worksheet.Cell(row, 10).Value = monitoring.Responsible;
                        worksheet.Cell(row, 11).Value = monitoring.Executive;
                        worksheet.Cell(row, 12).Value = monitoring.StatusName;
                        worksheet.Cell(row, 13).Value = monitoring.Quantity;
                        worksheet.Cell(row, 14).Value = monitoring.DaysToComplete;
                        worksheet.Cell(row, 15).Value = monitoring.RequirementDate?.ToString("yyyy-MM-dd") ?? "";
                        worksheet.Cell(row, 16).Value = monitoring.QuotedDate?.ToString("yyyy-MM-dd") ?? "";
                        worksheet.Cell(row, 17).Value = monitoring.ResponseDays ?? 0;
                        worksheet.Cell(row, 18).Value = monitoring.Contact;
                        worksheet.Cell(row, 19).Value = monitoring.Telephone;

                        for (int i = 2; i <= 19; i++)
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
                        var fileName = $"seguimientos_{currentTimePeru:yyyyMMdd_HHmmss}.xlsx";

                        return File(content, contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        
        [HttpPost("GetSunatValue")]
        public async Task<IActionResult> GetSunatValue(SunatModel obj)
        {
            try
            {
                string response = await _monitoringService.GetSunatValue(obj);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"error");
            }
        }

        [HttpPost("GetAllMonitorings")]
        public async Task<ActionResult<dynamic>> GetAllMonitorings(MonitoringPage pag)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _monitoringService.GetAllMonitorings(pag);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpGet("GetMonitoringById/{Id}")]
        public async Task<ActionResult<dynamic>> GetMonitoringById(Guid Id)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _monitoringService.GetMonitoringById(Id);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPost("CreateMonitoring")]
        public async Task<ActionResult<dynamic>> CreateMonitoring(AuxMonitoringModel obj)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _monitoringService.CreateMonitoring(obj);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPost("UpdateMonitoring/{Id}")]
        public async Task<ActionResult<dynamic>> UpdateMonitoring(Guid Id, AuxMonitoringModel obj)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _monitoringService.UpdateMonitoring(Id, obj);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPost("RemoveMonitoring/{Id}")]
        public async Task<ActionResult<dynamic>> RemoveMonitoring(Guid Id)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _monitoringService.RemoveMonitoring(Id);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPost("UpdateStatusMonitoring/{Id}/{OrderStatus}")]
        public async Task<ActionResult<dynamic>> UpdateStatusMonitoring(Guid Id, int OrderStatus)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _monitoringService.UpdateStatusMonitoring(Id, OrderStatus);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpGet("SummaryInfo")]
        public async Task<ActionResult<dynamic>> SummaryInfo()
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _monitoringService.SummaryInfo();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpGet("GetCommentById/{id}")]
        public async Task<ActionResult<dynamic>> GetCommentById(Guid id)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _monitoringService.GetCommentById(id);
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
                    _monitoringService.UpdateCommentById(id, newComment);
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("DuplicateMonitoring/{id}/{userId}")]
        public async Task<ActionResult<dynamic>> DuplicateMonitoring(Guid id, Guid userId)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _monitoringService.DuplicateMonitoring(id, userId);
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
