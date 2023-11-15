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

namespace DEMO_SOLDS.APP.Services
{
    public class InvoiceService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;


        public InvoiceService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        


        public async Task<dynamic> GetSunatValue(SunatModel obj)
        {
            try
            {
                SunatApi sunatApi = _configuration.GetSection("SunatApi").Get<SunatApi>();

                using (HttpClient httpClient = new HttpClient())
                {
                    string endpoint = sunatApi.Endpoint + obj.Name+"/"+obj.Code;
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sunatApi.TokenSunat);

                    // Realizar la solicitud GET
                    HttpResponseMessage response = await httpClient.GetAsync(endpoint);

                    // Verificar si la solicitud fue exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        // Leer y devolver la respuesta en formato dinámico
                        string responseBody = await response.Content.ReadAsStringAsync();
                        dynamic result = JsonConvert.DeserializeObject<dynamic>(responseBody);
                        string Name = result.nombres;
                        string firstLastName = result.apellidoPaterno;
                        string secondLastName = result.apellidoMaterno;
                        string fullName = Name + " " + firstLastName + " " +secondLastName;
                        return result.razonSocial ?? fullName??"";
                    }
                    else
                    {
                        // Manejar errores de la solicitud
                        throw new HttpRequestException($"Error en la solicitud: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el valor de Sunat", ex);
            }
        }
        public List<InvoiceModel> GetFilterDataForExcel(ExcelFilters obj)
        {
            obj.EndDate = obj.EndDate?.AddDays(+1);
            var invoicesList = _context.Invoices
                .Where(u => !u.IsDeleted && u.CreatedOn >= obj.StartDate && u.CreatedOn <= obj.EndDate)
                .OrderByDescending(u => u.CreatedOn)
                .Select(i => new InvoiceModel
                {
                    InvoiceCode = i.InvoiceCode,
                    IdentificationType = i.IdentificationType,
                    DocumentInfo = i.DocumentInfo,
                    IdentificationInfo = i.IdentificationInfo,
                    Telephone = i.Telephone,
                    Email = i.Email,
                    SelectedCategory = i.SelectedCategory,
                    DeliveryType = i.DeliveryType,
                    SelectedDistrict = i.SelectedDistrict,
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
                    Employee = i.Employee,
                    TotalOfPieces = i.TotalOfPieces,
                    UnitPiece = i.UnitPiece,
                    Contact = i.Contact,
                    UserId= i.UserId,
                })
                .ToList();

            return invoicesList;
        }
        public List<InvoiceModel> GetAllInvoices()
        {
            var userIdToPrefixMap = _context.AspNetUsers
                .Where(u => u.IsDeleted != true)
                .ToDictionary(u => u.Id, u => new { Prefix = u.Prefix, Name = u.Name, FirstLastName = u.FirstLastName });

            var invoicesList = _context.Invoices
                .Where(i => i.IsDeleted != true)
                .Select(i => new InvoiceModel
                {
                    Id = i.Id,
                    InvoiceCode = "COT-" + userIdToPrefixMap[i.UserId].Prefix + i.InvoiceCode.PadLeft(6, '0'),
                    IdentificationType = i.IdentificationType,
                    DocumentInfo = i.DocumentInfo,
                    IdentificationInfo = i.IdentificationInfo,
                    Telephone = i.Telephone,
                    Email = i.Email,
                    SelectedCategory = i.SelectedCategory,
                    SelectedMeasures = JsonConvert.DeserializeObject<List<string>>(i.SelectedMeasures),
                    MeasureQuantities = JsonConvert.DeserializeObject<List<int>>(i.MeasureQuantities),
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
                    CantParihuela= i.IsParihuelaNeeded == "No" ? 0: i.CantParihuela,
                })
                .ToList();

            return invoicesList;
        }

        public InvoiceModel GetInvoiceById(Guid Id)
        {
            var i = _context.Invoices
                .Where(u => u.IsDeleted != true && u.Id == Id)
                ?.FirstOrDefault();
            var userIdToPrefixMap = _context.AspNetUsers
                .Where(u => u.IsDeleted != true)
                .ToDictionary(u => u.Id, u => new { Prefix = u.Prefix, Name = u.Name, FirstLastName = u.FirstLastName });
            InvoiceModel invoiceById = new InvoiceModel
            {
                Id = i.Id,
                InvoiceCode = "COT-" + userIdToPrefixMap[i.UserId].Prefix + i.InvoiceCode.PadLeft(6, '0'),
                IdentificationType = i.IdentificationType,
                DocumentInfo = i.DocumentInfo,
                IdentificationInfo = i.IdentificationInfo,
                Telephone = i.Telephone,
                Email = i.Email,
                SelectedCategory = i.SelectedCategory,
                SelectedMeasures = JsonConvert.DeserializeObject<List<string>>(i.SelectedMeasures),
                MeasureQuantities = JsonConvert.DeserializeObject<List<int>>(i.MeasureQuantities),
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
                IsParihuelaNeeded= i.IsParihuelaNeeded ?? "No",
                CantParihuela= i.IsParihuelaNeeded=="No" ? 0 : i.CantParihuela,
                CostParihuela= i.IsParihuelaNeeded == "No" ? 0 : i.CostParihuela,
                TotalPriceParihuela= i.IsParihuelaNeeded == "No" ? 0 : i.TotalPriceParihuela,
                Address = i.Address,
                TotalOfPieces = i.TotalOfPieces,
                UnitPiece = i.UnitPiece,
                Contact = i.Contact,
                UserId = i.UserId
            };

            return invoiceById;
        }
        public void CreateCustomer(AuxInvoiceModel obj)
        {
            var newCustomer = new Customers
            {
                Id = Guid.NewGuid(),
                CustomerName = obj.IdentificationInfo,
                IdentificationType = obj.IdentificationType,
                IdentificationInfo = obj.DocumentInfo,
                IsDeleted = false
            };

            _context.Customers.Add(newCustomer);
            _context.SaveChanges();
        }
        public void DuplicateInvoice(Guid InvoiceId)
        {
            var obj = _context.Invoices.FirstOrDefault(x => x.Id == InvoiceId);
            if (obj != null)
            {
                var user = _context.AspNetUsers.SingleOrDefault(x => x.Email == obj.CreatedBy);

                string latestInvoiceNumber = _context.Invoices
                .OrderByDescending(i => i.CreatedOn)
                .Select(i => i.InvoiceCode)
                .FirstOrDefault() ?? string.Empty;
                int nextInvoiceNumber = 1;
                if (!string.IsNullOrEmpty(latestInvoiceNumber))
                {
                    nextInvoiceNumber = int.Parse(latestInvoiceNumber) + 1;
                }
                if (user != null)
                {
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
                        SelectedMeasures = obj.SelectedMeasures,
                        MeasureQuantities = obj.MeasureQuantities,
                        DeliveryType = obj.DeliveryType,
                        SelectedDistrict = obj.SelectedDistrict,
                        Truck20TN = obj.Truck20TN,
                        Truck32TN = obj.Truck32TN,
                        Truck9TN = obj.Truck9TN,
                        ProductsList = obj.ProductsList,
                        FleteList = obj.FleteList,
                        TotalWeight = obj.TotalWeight,
                        Subtotal = obj.Subtotal,
                        IgvRate = obj.IgvRate,
                        TotalInvoice = obj.TotalInvoice,
                        CreatedBy = obj.CreatedBy,
                        CreatedOn = DateTime.UtcNow,
                        LastUpdatedBy = obj.CreatedBy,
                        LastUpdatedOn = DateTime.UtcNow,
                        StatusOrder = 1,
                        StatusName = "En progreso",
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
                    };
                    _context.Add(newInvoice);
                    _context.SaveChanges();
                }
            }

        }
        public string CreateInvoice(AuxInvoiceModel obj)
        {
            var user = _context.AspNetUsers.SingleOrDefault(x => x.Email == obj.CreatedBy);

            string latestInvoiceNumber = _context.Invoices
            .OrderByDescending(i => i.CreatedOn)
            .Select(i => i.InvoiceCode)            
            .FirstOrDefault()??string.Empty;

            var existCustomer = _context.Customers.FirstOrDefault(x => x.IdentificationInfo == obj.DocumentInfo);
            if (existCustomer == null)
            {
                CreateCustomer(obj);
            }
            // Extract the numeric part and increment it
            int nextInvoiceNumber = 1;
            if (!string.IsNullOrEmpty(latestInvoiceNumber))
            {
                nextInvoiceNumber = int.Parse(latestInvoiceNumber) + 1;
            }
            if (user != null)
            {
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
                    CreatedBy = obj.CreatedBy,
                    CreatedOn = DateTime.UtcNow,
                    LastUpdatedBy = obj.CreatedBy,
                    LastUpdatedOn = DateTime.UtcNow,
                    StatusOrder = 1,
                    StatusName = "En progreso",
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
                };
                _context.Add(newInvoice);
                _context.SaveChanges();
            }

            return "Ok";
        }
        public string UpdateInvoice(Guid InvoiceId,AuxInvoiceModel obj)
        {
            Invoices? invoice = _context.Invoices.Where(x => x.IsDeleted!= true && x.Id == InvoiceId)?.FirstOrDefault();

            if (invoice != null)
            {
                invoice.IdentificationType = obj.IdentificationType;
                invoice.DocumentInfo = obj.DocumentInfo;
                invoice.IdentificationInfo = obj.IdentificationInfo;
                invoice.Telephone = obj.Telephone;
                invoice.Email = obj.Email;
                invoice.SelectedCategory = obj.SelectedCategory;
                invoice.SelectedMeasures = JsonConvert.SerializeObject(obj.SelectedMeasures);
                invoice.MeasureQuantities = JsonConvert.SerializeObject(obj.MeasureQuantities);
                invoice.DeliveryType = obj.DeliveryType;
                invoice.SelectedDistrict = obj.SelectedDistrict;
                invoice.Truck20TN = obj.Truck20TN;
                invoice.Truck32TN = obj.Truck32TN;
                invoice.Truck9TN = obj.Truck9TN;
                invoice.ProductsList = JsonConvert.SerializeObject(obj.ProductsList);
                invoice.FleteList = JsonConvert.SerializeObject(obj.FleteList);
                invoice.TotalWeight = obj.TotalWeight;
                invoice.Subtotal = obj.Subtotal;
                invoice.IgvRate = obj.IgvRate;
                invoice.TotalInvoice = obj.TotalInvoice;              
                invoice.LastUpdatedBy = obj.CreatedBy;
                invoice.LastUpdatedOn = DateTime.UtcNow;
                invoice.IsParihuelaNeeded = obj.IsParihuelaNeeded;
                invoice.CantParihuela = obj.CantParihuela;
                invoice.CostParihuela = obj.CostParihuela;
                invoice.TotalPriceParihuela = obj.TotalPriceParihuela;
                invoice.Address = obj.Address;
                invoice.UnitPiece = obj.UnitPiece;
                invoice.TotalOfPieces= obj.TotalOfPieces;
                invoice.Contact = obj.Contact;

                _context.SaveChanges();
            }

            return "Ok";
        }
        public string RemoveInvoice(Guid InvoiceId)
        {
            Invoices? invoice = _context.Invoices.Where(x => x.IsDeleted != true && x.Id == InvoiceId)?.FirstOrDefault();

            if (invoice != null)
            {
                invoice.IsDeleted = true;
                _context.SaveChanges();
            }
            else
            {
                return "Error";
            }

            return "Ok";
        }
        public string UpdateStatusInvoice(Guid InvoiceId, int OrderStatus)
        {
            Invoices? invoice = _context.Invoices.Where(x => x.IsDeleted != true && x.Id == InvoiceId)?.FirstOrDefault();

            if (invoice != null)
            {
                if(OrderStatus == 2)
                {
                    invoice.StatusOrder = 2;
                    invoice.StatusName = "Aprobada";
                }
                if(OrderStatus == 3)
                {
                    invoice.StatusOrder = 3;
                    invoice.StatusName = "Rechazada";
                }
                _context.SaveChanges();
            }
            else
            {
                return "Error";
            }

            return "Ok";
        }

        public dynamic SummaryInfo()
        {
            DateTime actualDay = DateTime.UtcNow;
            decimal monthGoal = 3200000;

            var data = _context.Invoices
                .Where(x => !x.IsDeleted && x.StatusOrder == 2);

            // Obtener un arreglo de precios facturados de cada mes
            decimal[] monthlyPrices = new decimal[12];
            decimal[] monthlyPricesLastYear = new decimal[12];

            foreach (var invoice in data.Where(u => u.CreatedOn.Year == actualDay.Year || u.CreatedOn.Year == actualDay.Year - 1))
            {
                int monthIndex = invoice.CreatedOn.Month - 1; // Restar 1 porque los índices de arreglo comienzan en 0
                decimal totalInvoice = invoice.TotalInvoice;

                if (invoice.CreatedOn.Year == actualDay.Year)
                    monthlyPrices[monthIndex] += totalInvoice;
                else if (invoice.CreatedOn.Year == actualDay.Year - 1)
                    monthlyPricesLastYear[monthIndex] += totalInvoice;
            }

            int numberOfInvoicesToday = data.Count(u => u.CreatedOn.Date == actualDay.Date);
            int numberOfInvoicesMonthly = data.Count(u => u.CreatedOn.Month == actualDay.Month);

            decimal totalToday = data
                .Where(u => u.CreatedOn.Date == actualDay.Date)
                .Sum(a => a.TotalInvoice);

            decimal totalMonth = monthlyPrices.Sum();

            var auxData = data.Where(u => u.CreatedOn.Month == actualDay.Month).ToList();
            var bloquesPercentage = CalculateCategoryPercentage(auxData, "BLOQUES", numberOfInvoicesMonthly);
            var adokingPercentage = CalculateCategoryPercentage(auxData, "ADO", numberOfInvoicesMonthly);
            var grassMichiPercentage = CalculateCategoryPercentage(auxData, "GR", numberOfInvoicesMonthly);
            var enchapePercentage = CalculateCategoryPercentage(auxData, "ENCHAPE", numberOfInvoicesMonthly);
            var aisladoresPercentage = CalculateCategoryPercentage(auxData, "AISLADORES", numberOfInvoicesMonthly);

            return new
            {
                ActualDay = actualDay,
                MonthGoal = monthGoal,
                NumberOfInvoicesToday = numberOfInvoicesToday,
                NumberOfInvoicesMonthly = numberOfInvoicesMonthly,
                TotalToday = totalToday,
                TotalMonth = totalMonth,
                PercentageTotalMonth = (totalMonth / monthGoal) * 100,
                MonthlyPrices = monthlyPrices,
                MonthlyPricesLastYear = monthlyPricesLastYear,
                PercentageProducts = new[] { bloquesPercentage, adokingPercentage, grassMichiPercentage, enchapePercentage, aisladoresPercentage }
            };
        }

        private decimal CalculateCategoryPercentage(List<Invoices> data, string category, int totalInvoices)
        {
            return (decimal)data.Count(x => x.SelectedCategory.Contains(category)) / totalInvoices;
        }





    }
}
