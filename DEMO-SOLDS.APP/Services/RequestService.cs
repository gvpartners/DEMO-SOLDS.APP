using Microsoft.Extensions.Configuration;
using R2.DEMO.APP.Context;
using DEMO_SOLDS.APP.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Web.Http.Results;
using ClosedXML.Excel;
using System.Linq;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.IdentityModel.Tokens;

namespace DEMO_SOLDS.APP.Services
{
    public class RequestService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;


        public RequestService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public InvoiceListResponse GetAllRequest(InvoicePage pag)
        {
            int recordsToSkip = (pag.PageNumber) * pag.PageSize;

            var userIdToPrefixMap = _context.AspNetUsers
                .ToDictionary(u => u.Id, u => new { Prefix = u.Prefix, Name = u.Name, FirstLastName = u.FirstLastName });
            var query = _context.Invoices.Where(i => i.IsDeleted != true && i.StatusOrder == 0);

            if (pag.Filters != null)
            {
                if (!string.IsNullOrEmpty(pag.Filters.IdentificationInfoFilter))
                {
                    query = query.Where(i => i.IdentificationInfo.Contains(pag.Filters.IdentificationInfoFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.IdentificationTypeFilter))
                {
                    query = query.Where(i => i.DocumentInfo.Contains(pag.Filters.IdentificationTypeFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.DocumentInfoFilter))
                {
                    query = query.Where(i => i.DocumentInfo.Contains(pag.Filters.DocumentInfoFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.TelephoneFilter))
                {
                    query = query.Where(i => i.Telephone.Contains(pag.Filters.TelephoneFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.SelectedCategoryFilter))
                {
                    query = query.Where(i => i.SelectedCategory.Contains(pag.Filters.SelectedCategoryFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.DeliveryTypeFilter))
                {
                    query = query.Where(i => i.DeliveryType.Contains(pag.Filters.DeliveryTypeFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.SelectedDistrictFilter))
                {
                    query = query.Where(i => i.SelectedDistrict.Contains(pag.Filters.SelectedDistrictFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.TotalInvoiceFilter))
                {
                    query = query.Where(i => i.TotalInvoice.ToString().Contains(pag.Filters.TotalInvoiceFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.StatusNameFilter))
                {
                    query = query.Where(i => i.StatusName.Contains(pag.Filters.StatusNameFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.InvoiceCodeFilter))
                {
                    query = query.Where(i => i.InvoiceCode.Contains(pag.Filters.InvoiceCodeFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.TotalOfPieces))
                {
                    query = query.Where(i => i.TotalOfPieces.ToString().Contains(pag.Filters.TotalOfPieces));
                }

                if (!string.IsNullOrEmpty(pag.Filters.AddressFilter))
                {
                    query = query.Where(i => i.Address.Contains(pag.Filters.AddressFilter));
                }
                if (!string.IsNullOrEmpty(pag.Filters.EmployeeFilter))
                {
                    query = query.Where(i => i.Employee.Contains(pag.Filters.EmployeeFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.UnitPieceFilter))
                {
                    query = query.Where(i => i.UnitPiece.Contains(pag.Filters.UnitPieceFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.ContactFilter))
                {
                    query = query.Where(i => i.Contact.Contains(pag.Filters.ContactFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.ReferenceFilter))
                {
                    query = query.Where(i => i.Reference.Contains(pag.Filters.ReferenceFilter));
                }
                if (pag.Filters.InvoiceDate.HasValue)
                {
                    query = query.Where(i => i.CreatedOn.Date == pag.Filters.InvoiceDate.Value.Date);
                }
            }
            query = query
                .OrderByDescending(i => i.CreatedOn);
            var totalOfInvoices = query.Count();
            var invoicesList = query
                .Skip(recordsToSkip)
                .Take(pag.PageSize)
                .Select(i => new InvoiceModel
                {
                    Id = i.Id,
                    InvoiceCode = "COT-" + userIdToPrefixMap[i.UserId].Prefix + i.InvoiceCode.PadLeft(7, '0'),
                    IdentificationType = i.IdentificationType,
                    DocumentInfo = i.DocumentInfo,
                    IdentificationInfo = i.IdentificationInfo,
                    Telephone = i.Telephone,
                    Email = i.Email,
                    SelectedCategory = i.SelectedCategory,
                    SelectedMeasures = JsonConvert.DeserializeObject<List<string>>(i.SelectedMeasures),
                    MeasureQuantities = JsonConvert.DeserializeObject<List<decimal>>(i.MeasureQuantities),
                    DeliveryType = i.DeliveryType,
                    SelectedDistrict = i.SelectedDistrict,
                    Truck9TN = i.Truck9TN,
                    Truck20TN = i.Truck20TN,
                    Truck32TN = i.Truck32TN,
                    ProductsList = JsonConvert.DeserializeObject<List<ProductModel>>(i.ProductsList),
                    FleteList = JsonConvert.DeserializeObject<List<TruckModel>>(i.FleteList),
                    TotalWeight = i.TotalWeight,
                    Subtotal = i.Subtotal,
                    IgvRate = i.IgvRate,
                    TotalInvoice = i.TotalInvoice,
                    CreatedBy = i.CreatedBy,
                    CreatedOn = i.CreatedOn,
                    LastUpdatedBy = i.LastUpdatedBy,
                    LastUpdatedOn = i.LastUpdatedOn,
                    StatusOrder = i.StatusOrder,
                    StatusName = i.StatusName,
                    Address = i.Address,
                    Employee = userIdToPrefixMap[i.UserId].Name + " " + userIdToPrefixMap[i.UserId].FirstLastName,
                    TotalOfPieces = i.TotalOfPieces,
                    UnitPiece = i.UnitPiece,
                    Contact = i.Contact,
                    UserId = i.UserId,
                    CantParihuela = i.CantParihuela != null ? i.CantParihuela : 0,
                    Comment = i.Comment,
                    Reference = i.Reference,
                    IsParihuelaNeeded = i.IsParihuelaNeeded,
                })
                .ToList();

            // Crear y devolver la respuesta con la lista paginada de facturas y el total de facturas
            return new InvoiceListResponse
            {
                Total = totalOfInvoices,
                Invoices = invoicesList
            };
        }

    }
}
