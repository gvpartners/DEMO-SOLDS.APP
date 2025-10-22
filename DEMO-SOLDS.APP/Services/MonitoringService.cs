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
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DEMO_SOLDS.APP.Services
{
    public class MonitoringService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public MonitoringService(IConfiguration configuration, AppDbContext context)
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
                    string endpoint = sunatApi.Endpoint + obj.Name?.ToLower() + "/" + obj.Code;
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sunatApi.TokenSunat);

                    // Realizar la solicitud GET
                    HttpResponseMessage response = await httpClient.GetAsync(endpoint);

                    // Verificar si la solicitud fue exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        // Leer y devolver la respuesta en formato din�mico
                        string responseBody = await response.Content.ReadAsStringAsync();
                        dynamic result = JsonConvert.DeserializeObject<dynamic>(responseBody);
                        string Name = result.nombres;
                        string firstLastName = result.apellidoPaterno;
                        string secondLastName = result.apellidoMaterno;
                        string fullName = Name + " " + firstLastName + " " + secondLastName;
                        return result.razonSocial ?? fullName ?? "";
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

        public List<MonitoringModel> GetFilterDataForExcel(ExcelFilters obj)
        {
            if (obj.StartDate.HasValue && obj.EndDate.HasValue)
            {
                var monitoringsList = _context.Monitoring
                    .Where(u => !u.IsDeleted &&
                                 u.RequirementDate.HasValue &&
                                 u.RequirementDate.Value.Date >= obj.StartDate.Value.Date &&
                                 u.RequirementDate.Value.Date <= obj.EndDate.Value.Date)
                    .OrderByDescending(u => u.RequirementDate)
                    .Select(m => new MonitoringModel
                    {
                        MonitoringCode = m.MonitoringCode,
                        IdentificationType = m.IdentificationType,
                        DocumentInfo = m.DocumentInfo,
                        IdentificationInfo = m.IdentificationInfo,
                        Telephone = m.Telephone,
                        Email = m.Email,
                        Contact = m.Contact,
                        Quantity = m.Quantity,
                         SelectedDistrict = m.SelectedDistrict,
                         SelectedCategory = m.SelectedCategory,
                         DaysToComplete = m.DaysToComplete,
                         DeliveryType = m.DeliveryType,
                         RequirementDate = m.RequirementDate,
                        QuotedDate = m.QuotedDate,
                        ResponseDays = m.ResponseDays,
                        StatusOrder = m.StatusOrder,
                        StatusName = m.StatusName,
                        Responsible = m.Responsible,
                        Executive = m.Executive,
                        Segment = m.Segment,
                        Address = m.Address,
                        Comment = m.Comment,
                        CreatedBy = m.CreatedBy,
                        CreatedOn = m.CreatedOn,
                        LastUpdatedBy = m.LastUpdatedBy,
                        LastUpdatedOn = m.LastUpdatedOn,
                        UserId = m.UserId
                    })
                    .ToList();

                return monitoringsList;
            }

            return new List<MonitoringModel>();
        }

        public MonitoringListResponse GetAllMonitorings(MonitoringPage pag)
        {
            int recordsToSkip = (pag.PageNumber) * pag.PageSize;

            var userIdToPrefixMap = _context.AspNetUsers
                .ToDictionary(u => u.Id, u => new { Prefix = u.Prefix, Name = u.Name, FirstLastName = u.FirstLastName });
            
            var query = _context.Monitoring.Where(m => m.IsDeleted != true);

            if (pag.Filters != null)
            {
                if (!string.IsNullOrEmpty(pag.Filters.MonitoringCodeFilter))
                {
                    query = query.Where(m => m.MonitoringCode.Contains(pag.Filters.MonitoringCodeFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.DocumentInfoFilter))
                {
                    query = query.Where(m => m.DocumentInfo.Contains(pag.Filters.DocumentInfoFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.ContactFilter))
                {
                    query = query.Where(m => m.Contact.Contains(pag.Filters.ContactFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.TelephoneFilter))
                {
                    query = query.Where(m => m.Telephone.Contains(pag.Filters.TelephoneFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.QuantityFilter))
                {
                    query = query.Where(m => m.Quantity.ToString().Contains(pag.Filters.QuantityFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.SelectedDistrictFilter))
                {
                    query = query.Where(m => m.SelectedDistrict.Contains(pag.Filters.SelectedDistrictFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.SelectedCategoryFilter))
                {
                    query = query.Where(m => m.SelectedCategory.Contains(pag.Filters.SelectedCategoryFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.DaysToCompleteFilter))
                {
                    query = query.Where(m => m.DaysToComplete.ToString().Contains(pag.Filters.DaysToCompleteFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.DeliveryTypeFilter))
                {
                    query = query.Where(m => m.DeliveryType.Contains(pag.Filters.DeliveryTypeFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.StatusNameFilter))
                {
                    query = query.Where(m => m.StatusName.Contains(pag.Filters.StatusNameFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.ResponsibleFilter))
                {
                    query = query.Where(m => m.Responsible.Contains(pag.Filters.ResponsibleFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.ExecutiveFilter))
                {
                    query = query.Where(m => m.Executive.Contains(pag.Filters.ExecutiveFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.SegmentFilter))
                {
                    query = query.Where(m => m.Segment.Contains(pag.Filters.SegmentFilter));
                }

                if (pag.Filters.RequirementDateFilter.HasValue)
                {
                    query = query.Where(m => m.RequirementDate.HasValue && 
                                           m.RequirementDate.Value.Date == pag.Filters.RequirementDateFilter.Value.Date);
                }

                if (pag.Filters.QuotedDateFilter.HasValue)
                {
                    query = query.Where(m => m.QuotedDate.HasValue && 
                                           m.QuotedDate.Value.Date == pag.Filters.QuotedDateFilter.Value.Date);
                }

                if (!string.IsNullOrEmpty(pag.Filters.SearchFilter))
                {
                    query = query.Where(m => 
                        m.MonitoringCode.Contains(pag.Filters.SearchFilter) ||
                        m.DocumentInfo.Contains(pag.Filters.SearchFilter) ||
                        m.IdentificationInfo.Contains(pag.Filters.SearchFilter) ||
                        m.Contact.Contains(pag.Filters.SearchFilter) ||
                        m.Telephone.Contains(pag.Filters.SearchFilter));
                }
            }

            var totalRecords = query.Count();

            var monitorings = query
                .OrderByDescending(m => m.CreatedOn)
                .Skip(recordsToSkip)
                .Take(pag.PageSize)
                .Select(m => new MonitoringModel
                {
                    Id = m.Id,
                    MonitoringCode = m.MonitoringCode,
                    IdentificationType = m.IdentificationType,
                    DocumentInfo = m.DocumentInfo,
                    IdentificationInfo = m.IdentificationInfo,
                    Telephone = m.Telephone,
                    Email = m.Email,
                    Contact = m.Contact,
                    Quantity = m.Quantity,
                         SelectedDistrict = m.SelectedDistrict,
                         SelectedCategory = m.SelectedCategory,
                         DaysToComplete = m.DaysToComplete,
                         DeliveryType = m.DeliveryType,
                         RequirementDate = m.RequirementDate,
                    QuotedDate = m.QuotedDate,
                    ResponseDays = m.ResponseDays,
                    StatusOrder = m.StatusOrder,
                    StatusName = m.StatusName,
                    Responsible = m.Responsible,
                    Executive = m.Executive,
                    Segment = m.Segment,
                    Address = m.Address,
                    Comment = m.Comment,
                    CreatedBy = m.CreatedBy,
                    CreatedOn = m.CreatedOn,
                    LastUpdatedBy = m.LastUpdatedBy,
                    LastUpdatedOn = m.LastUpdatedOn,
                    UserId = m.UserId,
                    IsDeleted = m.IsDeleted
                })
                .ToList();

            return new MonitoringListResponse
            {
                Monitorings = monitorings,
                Total = totalRecords
            };
        }

        public MonitoringModel GetMonitoringById(Guid id)
        {
            var monitoring = _context.Monitoring
                .Where(m => m.Id == id && !m.IsDeleted)
                .Select(m => new MonitoringModel
                {
                    Id = m.Id,
                    MonitoringCode = m.MonitoringCode,
                    IdentificationType = m.IdentificationType,
                    DocumentInfo = m.DocumentInfo,
                    IdentificationInfo = m.IdentificationInfo,
                    Telephone = m.Telephone,
                    Email = m.Email,
                    Contact = m.Contact,
                    Quantity = m.Quantity,
                         SelectedDistrict = m.SelectedDistrict,
                         SelectedCategory = m.SelectedCategory,
                         DaysToComplete = m.DaysToComplete,
                         DeliveryType = m.DeliveryType,
                         RequirementDate = m.RequirementDate,
                    QuotedDate = m.QuotedDate,
                    ResponseDays = m.ResponseDays,
                    StatusOrder = m.StatusOrder,
                    StatusName = m.StatusName,
                    Responsible = m.Responsible,
                    Executive = m.Executive,
                    Segment = m.Segment,
                    Address = m.Address,
                    Comment = m.Comment,
                    CreatedBy = m.CreatedBy,
                    CreatedOn = m.CreatedOn,
                    LastUpdatedBy = m.LastUpdatedBy,
                    LastUpdatedOn = m.LastUpdatedOn,
                    UserId = m.UserId,
                    IsDeleted = m.IsDeleted
                })
                .FirstOrDefault();

            return monitoring;
        }

        public MonitoringModel CreateMonitoring(AuxMonitoringModel obj)
        {
            try
            {
                // Obtener la zona horaria de Perú
                TimeZoneInfo peruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                // Obtener la hora actual en la zona horaria de Perú
                DateTime currentTimePeru = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, peruTimeZone);

                var user = _context.AspNetUsers.FirstOrDefault(u => u.Id == obj.UserId);
                if (user == null)
                {
                    throw new Exception("Usuario no encontrado");
                }

                // Generate new monitoring code
                var lastNum = _context.Monitoring.Count();

                int nextNumber = lastNum + 1;

                string monitoringCode = $"SEG-{user.Prefix}-{nextNumber.ToString().PadLeft(6, '0')}";

                // Calculate ResponseDays
                int responseDays = 0;
                if (obj.RequirementDate.HasValue && obj.QuotedDate.HasValue)
                {
                    responseDays = (int)(obj.QuotedDate.Value - obj.RequirementDate.Value).TotalDays;
                }

                var newMonitoring = new Monitoring
                {
                    Id = Guid.NewGuid(),
                    MonitoringCode = monitoringCode,
                    IdentificationType = obj.IdentificationType,
                    DocumentInfo = obj.DocumentInfo,
                    IdentificationInfo = obj.IdentificationInfo,
                    Telephone = obj.Telephone,
                    Email = obj.Email,
                    Contact = obj.Contact,
                    Quantity = obj.Quantity,
                     SelectedDistrict = obj.SelectedDistrict,
                     SelectedCategory = obj.SelectedCategory,
                     DaysToComplete = obj.DaysToComplete,
                     DeliveryType = obj.DeliveryType,
                     RequirementDate = obj.RequirementDate,
                    QuotedDate = obj.QuotedDate,
                    ResponseDays = responseDays,
                    StatusOrder = 1,
                    StatusName = "En seguimiento",
                    Responsible = obj.Responsible,
                    Executive = obj.Executive,
                    Segment = obj.Segment,
                    Address = obj.Address,
                    Comment = obj.Comment,
                    CreatedBy = user.Name + " " + user.FirstLastName,
                    CreatedOn = currentTimePeru,
                    LastUpdatedBy = user.Name + " " + user.FirstLastName,
                    LastUpdatedOn = currentTimePeru,
                    UserId = obj.UserId,
                    IsDeleted = false
                };

                _context.Monitoring.Add(newMonitoring);
                _context.SaveChanges();

                return GetMonitoringById(newMonitoring.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear el seguimiento: {ex.Message}");
            }
        }

        public MonitoringModel UpdateMonitoring(Guid id, AuxMonitoringModel obj)
        {
            try
            {
                // Obtener la zona horaria de Perú
                TimeZoneInfo peruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                // Obtener la hora actual en la zona horaria de Perú
                DateTime currentTimePeru = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, peruTimeZone);

                var monitoring = _context.Monitoring.FirstOrDefault(m => m.Id == id && !m.IsDeleted);
                if (monitoring == null)
                {
                    throw new Exception("Seguimiento no encontrado");
                }

                var user = _context.AspNetUsers.FirstOrDefault(u => u.Id == obj.UserId);
                if (user == null)
                {
                    throw new Exception("Usuario no encontrado");
                }

                monitoring.IdentificationType = obj.IdentificationType;
                monitoring.DocumentInfo = obj.DocumentInfo;
                monitoring.IdentificationInfo = obj.IdentificationInfo;
                monitoring.Telephone = obj.Telephone;
                monitoring.Email = obj.Email;
                monitoring.Contact = obj.Contact;
                monitoring.Quantity = obj.Quantity;
                monitoring.SelectedDistrict = obj.SelectedDistrict;
                monitoring.SelectedCategory = obj.SelectedCategory;
                monitoring.DaysToComplete = obj.DaysToComplete;
                monitoring.RequirementDate = obj.RequirementDate;
                monitoring.QuotedDate = obj.QuotedDate;
                monitoring.DeliveryType = obj.DeliveryType;
                
                // Calculate ResponseDays
                int responseDays = 0;
                if (obj.RequirementDate.HasValue && obj.QuotedDate.HasValue)
                {
                    responseDays = (int)(obj.QuotedDate.Value - obj.RequirementDate.Value).TotalDays;
                }
                monitoring.ResponseDays = responseDays;
                
                monitoring.Responsible = obj.Responsible;
                monitoring.Executive = obj.Executive;
                monitoring.Segment = obj.Segment;
                monitoring.Address = obj.Address;
                monitoring.Comment = obj.Comment;
                monitoring.LastUpdatedBy = user.Name + " " + user.FirstLastName;
                monitoring.LastUpdatedOn = currentTimePeru;

                _context.SaveChanges();

                return GetMonitoringById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el seguimiento: {ex.Message}");
            }
        }

        public bool RemoveMonitoring(Guid id)
        {
            try
            {
                // Obtener la zona horaria de Perú
                TimeZoneInfo peruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                // Obtener la hora actual en la zona horaria de Perú
                DateTime currentTimePeru = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, peruTimeZone);

                var monitoring = _context.Monitoring.FirstOrDefault(m => m.Id == id && !m.IsDeleted);
                if (monitoring == null)
                {
                    throw new Exception("Seguimiento no encontrado");
                }

                monitoring.IsDeleted = true;
                monitoring.LastUpdatedOn = currentTimePeru;

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar el seguimiento: {ex.Message}");
            }
        }

        public MonitoringModel UpdateStatusMonitoring(Guid id, int orderStatus)
        {
            try
            {
                // Obtener la zona horaria de Perú
                TimeZoneInfo peruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                // Obtener la hora actual en la zona horaria de Perú
                DateTime currentTimePeru = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, peruTimeZone);

                var monitoring = _context.Monitoring.FirstOrDefault(m => m.Id == id && !m.IsDeleted);
                if (monitoring == null)
                {
                    throw new Exception("Seguimiento no encontrado");
                }

                string statusName = orderStatus switch
                {
                    1 => "En seguimiento",
                    2 => "Cotizado",
                    3 => "Cerrado",
                    4 => "Rechazado",
                    _ => "En seguimiento"
                };

                monitoring.StatusOrder = orderStatus;
                monitoring.StatusName = statusName;
                monitoring.LastUpdatedOn = currentTimePeru;

                _context.SaveChanges();

                return GetMonitoringById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el estado del seguimiento: {ex.Message}");
            }
        }

        public string GetCommentById(Guid id)
        {
            var monitoring = _context.Monitoring
                .Where(m => m.Id == id && !m.IsDeleted)
                .Select(m => m.Comment)
                .FirstOrDefault();

            return monitoring ?? "";
        }

        public bool UpdateCommentById(Guid id, string newComment)
        {
            try
            {
                // Obtener la zona horaria de Perú
                TimeZoneInfo peruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                // Obtener la hora actual en la zona horaria de Perú
                DateTime currentTimePeru = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, peruTimeZone);

                var monitoring = _context.Monitoring.FirstOrDefault(m => m.Id == id && !m.IsDeleted);
                if (monitoring == null)
                {
                    throw new Exception("Seguimiento no encontrado");
                }

                monitoring.Comment = newComment;
                monitoring.LastUpdatedOn = currentTimePeru;

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el comentario: {ex.Message}");
            }
        }

        public dynamic SummaryInfo()
        {
            var totalMonitorings = _context.Monitoring.Count(m => !m.IsDeleted);
            var monitoringsInProgress = _context.Monitoring.Count(m => !m.IsDeleted && m.StatusOrder == 1);
            var monitoringsQuoted = _context.Monitoring.Count(m => !m.IsDeleted && m.StatusOrder == 2);
            var monitoringsClosed = _context.Monitoring.Count(m => !m.IsDeleted && m.StatusOrder == 3);
            var monitoringsRejected = _context.Monitoring.Count(m => !m.IsDeleted && m.StatusOrder == 4);

            return new
            {
                TotalMonitorings = totalMonitorings,
                MonitoringsInProgress = monitoringsInProgress,
                MonitoringsQuoted = monitoringsQuoted,
                MonitoringsClosed = monitoringsClosed,
                MonitoringsRejected = monitoringsRejected
            };
        }

        public MonitoringModel DuplicateMonitoring(Guid id, Guid userId)
        {
            try
            {
                // Obtener la zona horaria de Perú
                TimeZoneInfo peruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                // Obtener la hora actual en la zona horaria de Perú
                DateTime currentTimePeru = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, peruTimeZone);

                // Get the original monitoring
                var originalMonitoring = _context.Monitoring.FirstOrDefault(m => m.Id == id && !m.IsDeleted);
                if (originalMonitoring == null)
                {
                    throw new Exception("Seguimiento no encontrado");
                }

                // Get user information
                var user = _context.AspNetUsers.FirstOrDefault(u => u.Id == userId && !u.IsDeleted);
                if (user == null)
                {
                    throw new Exception("Usuario no encontrado");
                }

                // Generate new monitoring code
                var lastNum = _context.Monitoring.Count();

                int nextNumber = lastNum + 1;

                string monitoringCode = $"SEG-{user.Prefix}-{nextNumber.ToString().PadLeft(6, '0')}";

                // Calculate ResponseDays
                int responseDays = 0;
                if (originalMonitoring.RequirementDate.HasValue && originalMonitoring.QuotedDate.HasValue)
                {
                    responseDays = (int)(originalMonitoring.QuotedDate.Value - originalMonitoring.RequirementDate.Value).TotalDays;
                }

                // Create new monitoring
                var newMonitoring = new Monitoring
                {
                    Id = Guid.NewGuid(),
                    MonitoringCode = monitoringCode,
                    IdentificationType = originalMonitoring.IdentificationType,
                    DocumentInfo = originalMonitoring.DocumentInfo,
                    IdentificationInfo = originalMonitoring.IdentificationInfo,
                    Telephone = originalMonitoring.Telephone,
                    Email = originalMonitoring.Email,
                    Contact = originalMonitoring.Contact,
                    Quantity = originalMonitoring.Quantity,
                    SelectedDistrict = originalMonitoring.SelectedDistrict,
                    SelectedCategory = originalMonitoring.SelectedCategory,
                    DaysToComplete = originalMonitoring.DaysToComplete,
                    DeliveryType = originalMonitoring.DeliveryType,
                    RequirementDate = originalMonitoring.RequirementDate,
                    QuotedDate = originalMonitoring.QuotedDate,
                    ResponseDays = responseDays,
                    StatusOrder = 1, // Reset to "En seguimiento"
                    StatusName = "En seguimiento",
                    Responsible = originalMonitoring.Responsible,
                    Executive = originalMonitoring.Executive,
                    Segment = originalMonitoring.Segment,
                    Address = originalMonitoring.Address,
                    Comment = "",
                    CreatedBy = user.Name + " " + user.FirstLastName,
                    CreatedOn = currentTimePeru,
                    LastUpdatedBy = user.Name + " " + user.FirstLastName,
                    LastUpdatedOn = currentTimePeru,
                    UserId = userId,
                    IsDeleted = false
                };

                _context.Monitoring.Add(newMonitoring);
                _context.SaveChanges();

                return GetMonitoringById(newMonitoring.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al duplicar el seguimiento: {ex.Message}");
            }
        }
    }
}
