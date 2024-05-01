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
    public class InvoiceController : ControllerBase
    {
        private UserService _userService;
        private InvoiceService _invoiceService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext context;

        public InvoiceController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            this.context = context;
            _userService = new UserService(configuration, context);
            _invoiceService = new InvoiceService(configuration, context);
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
                    var worksheet = workbook.Worksheets.Add("REGISTRO DE COTIZACIONES");
                    worksheet.ShowGridLines = false;

                    var data = _invoiceService.GetFilterDataForExcel(obj);

                    string[] headers = { 
                        "FECHA",
                        "# COT",
                        "CLIENTE",
                        "RUC/DNI",
                        "CATEGORIA",
                        "PRODUCTO",
                        "CANTIDAD",
                        "U.M",
                        "TOTAL S/",
                        "ENTREGA",
                        "DIRECCIÓN",
                        "DISTRITO",
                        "REFERENCIA",
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

                    worksheet.Range("B2:R2").SetAutoFilter();

                    int row = 3;
                    foreach (var invoice in data)
                    {
                        if(invoice.ProductsList != null && invoice.ProductsList.Any())
                        {
                            foreach (var product in invoice.ProductsList)
                            {
                                worksheet.Cell(row, 2).Value = invoice.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss");
                                worksheet.Cell(row, 3).Value = "COT-" + userIdToPrefixMap[invoice.UserId].Prefix + invoice?.InvoiceCode?.PadLeft(6, '0');
                                worksheet.Cell(row, 4).Value = invoice?.IdentificationInfo;
                                worksheet.Cell(row, 5).Value = invoice?.DocumentInfo;
                                worksheet.Cell(row, 6).Value = invoice?.SelectedCategory;
                                worksheet.Cell(row, 7).Value = product?.Product;
                                worksheet.Cell(row, 8).Value = product?.Quantity;
                                worksheet.Cell(row, 9).Value = invoice?.UnitPiece;
                                // Formatear la celda como moneda en soles
                                worksheet.Cell(row, 10).Style.NumberFormat.Format = "\"S/\" #,##0.00";
                                worksheet.Cell(row, 10).Value = product?.TotalPrice??0.0m;
                                worksheet.Cell(row, 11).Value = invoice?.DeliveryType;
                                worksheet.Cell(row, 12).Value = invoice?.Address;
                                worksheet.Cell(row, 13).Value = invoice?.SelectedDistrict;
                                worksheet.Cell(row, 14).Value = invoice?.Reference;
                                worksheet.Cell(row, 15).Value = invoice?.Contact;
                                worksheet.Cell(row, 16).Value = invoice?.Telephone;
                                worksheet.Cell(row, 17).Value = userIdToPrefixMap[invoice.UserId].Name + " " + userIdToPrefixMap[invoice.UserId].FirstLastName;
                                worksheet.Cell(row, 18).Value = invoice?.StatusName;

                                for (int i = 2; i <= 18; i++)
                                {
                                    worksheet.Cell(row, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                    worksheet.Cell(row, i).Style.Border.OutsideBorderColor = XLColor.Black;
                                    worksheet.Cell(row, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                    worksheet.Column(i).AdjustToContents();
                                }

                                row++;
                            }
                            
                        }
                        
                    }

                    using (var originalStream = new MemoryStream())
                    {
                        workbook.SaveAs(originalStream);
                        var content = new MemoryStream(originalStream.ToArray());

                        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        var fileName = "invoice.xlsx";

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
                string response = await _invoiceService.GetSunatValue(obj);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"error");
            }
        }

        [HttpPost("GetAllInvoices")]
        public async Task<ActionResult<dynamic>> GetAllInvoices(InvoicePage pag)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _invoiceService.GetAllInvoices(pag);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }
        [HttpGet("GetInvoiceById/{Id}")]
        public async Task<ActionResult<dynamic>> GetInvoiceById(Guid Id)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _invoiceService.GetInvoiceById(Id);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }
        [HttpPost("CreateInvoice")]
        public async Task<ActionResult<dynamic>> CreateInvoice(AuxInvoiceModel obj)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _invoiceService.CreateInvoice(obj);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPost("UpdateInvoice/{Id}")]
        public async Task<ActionResult<dynamic>> UpdateInvoice(Guid Id,AuxInvoiceModel obj)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _invoiceService.UpdateInvoice(Id,obj);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }
        [HttpPost("RemoveInvoice/{Id}")]
        public async Task<ActionResult<dynamic>> RemoveInvoice(Guid Id)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _invoiceService.RemoveInvoice(Id);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPost("UpdateStatusInvoice/{Id}/{OrderStatus}")]
        public async Task<ActionResult<dynamic>> UpdateStatusInvoice(Guid Id, int OrderStatus)
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _invoiceService.UpdateStatusInvoice(Id, OrderStatus);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return Ok(response);
            });
        }

        [HttpPost("DuplicateInvoice/{id}/{userId}")]
        public async Task<IActionResult> DuplicateInvoice(Guid id,Guid userId)
        {
            try
            {
                await Task.Run(() =>
                {
                    _invoiceService.DuplicateInvoice(id,userId);
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("SummaryInfo")]
        public async Task<ActionResult<dynamic>> SummaryInfo()
        {
            dynamic response;
            return await Task.Run(() =>
            {
                try
                {
                    response = _invoiceService.SummaryInfo();
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
                    response = _invoiceService.GetCommentById(id);
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
                    _invoiceService.UpdateCommentbyId(id,newComment);
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
