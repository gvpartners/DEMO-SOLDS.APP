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
    public class TrackService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;


        public TrackService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public TrackListResponse GetAllTracks(TrackPage pag)
        {
            int recordsToSkip = (pag.PageNumber) * pag.PageSize;

            var userIdToPrefixMap = _context.AspNetUsers
                .ToDictionary(u => u.Id, u => new { Prefix = u.Prefix, Name = u.Name, FirstLastName = u.FirstLastName });
            var query = _context.Track.Where(i => i.IsDeleted != true);

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
                                
                if (!string.IsNullOrEmpty(pag.Filters.StatusNameFilter))
                {
                    query = query.Where(i => i.StatusName.Contains(pag.Filters.StatusNameFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.TrackCodeFilter))
                {
                    query = query.Where(i => i.TrackCode.Contains(pag.Filters.TrackCodeFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.TotalOfPieces))
                {
                    query = query.Where(i => i.TotalOfPieces.ToString().Contains(pag.Filters.TotalOfPieces));
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

                if (pag.Filters.TrackDate.HasValue)
                {
                    query = query.Where(i => i.CreatedOn.Date == pag.Filters.TrackDate.Value.Date);
                }
                if (pag.Filters.DeliveryDate.HasValue)
                {
                    query = query.Where(i => i.DeliveryDate.Date == pag.Filters.DeliveryDate.Value.Date);
                }
                if (pag.Filters.OrderDate.HasValue)
                {
                    query = query.Where(i => i.OrderDate.Date == pag.Filters.OrderDate.Value.Date);
                }
            }
            query = query.OrderByDescending(i => i.CreatedOn);
            var totalOfTracks = query.Count();
            var tracksList = query
                .Skip(recordsToSkip)
                .Take(pag.PageSize)
                .Select(i => new AuxTrackModel
                {
                    Id = i.Id,
                    TrackCode = "PD-" + userIdToPrefixMap[i.UserId].Prefix + i.TrackCode.PadLeft(7, '0'),
                    IdentificationType = i.IdentificationType,
                    DocumentInfo = i.DocumentInfo,
                    IdentificationInfo = i.IdentificationInfo,
                    Telephone = i.Telephone,
                    SelectedCategory = i.SelectedCategory,
                    SelectedMeasures = JsonConvert.DeserializeObject<List<string>>(i.SelectedMeasures),
                    MeasureQuantities = JsonConvert.DeserializeObject<List<decimal>>(i.MeasureQuantities),
                    DeliveryType = i.DeliveryType,
                    TotalWeight = i.TotalWeight,
                    CreatedBy = i.CreatedBy,
                    CreatedOn = i.CreatedOn,
                    LastUpdatedBy = i.LastUpdatedBy,
                    LastUpdatedOn = i.LastUpdatedOn,
                    StatusOrder = i.StatusOrder,
                    StatusName = i.StatusName,
                    Employee = userIdToPrefixMap[i.UserId].Name + " " + userIdToPrefixMap[i.UserId].FirstLastName,
                    TotalOfPieces = i.TotalOfPieces,
                    UnitPiece = i.UnitPiece,
                    Contact = i.Contact,
                    UserId = i.UserId,
                    DeliveryDate = i.DeliveryDate,
                    OrderDate = i.OrderDate
                })
                .ToList();

            return new TrackListResponse
            {
                Total = totalOfTracks,
                Tracks = tracksList
            };
        }
        public string CreateTrack(TrackModel obj)
        {
            var user = _context.AspNetUsers.SingleOrDefault(x => x.Email == obj.CreatedBy);
            TimeZoneInfo peruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

            DateTime currentTimePeru = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, peruTimeZone);

            string latestTrackNumber = _context.Track
            .OrderByDescending(i => i.CreatedOn)
            .Select(i => i.TrackCode)
            .FirstOrDefault() ?? string.Empty;

            var existCustomer = _context.Customers.FirstOrDefault(x => x.IdentificationInfo == obj.DocumentInfo);
            if (existCustomer == null)
            {
                CreateCustomer(obj);
            }
            int nextTrackNumber = 1;
            if (!string.IsNullOrEmpty(latestTrackNumber))
            {
                nextTrackNumber = int.Parse(latestTrackNumber) + 1;
            }
            if (user != null)
            {
                Tracks newTrack = new()
                {
                    Id = Guid.NewGuid(),
                    TrackCode = nextTrackNumber.ToString(),
                    IdentificationType = obj.IdentificationType,
                    DocumentInfo = obj.DocumentInfo,
                    IdentificationInfo = obj.IdentificationInfo,
                    Telephone = obj.Telephone,
                    SelectedCategory = obj.SelectedCategory,
                    SelectedMeasures = JsonConvert.SerializeObject(obj.SelectedMeasures),
                    MeasureQuantities = JsonConvert.SerializeObject(obj.MeasureQuantities),
                    DeliveryType = obj.DeliveryType,
                    TotalWeight = obj.TotalWeight,
                    CreatedBy = obj.CreatedBy,
                    CreatedOn = currentTimePeru,
                    LastUpdatedBy = obj.CreatedBy,
                    LastUpdatedOn = currentTimePeru,
                    StatusOrder = 1,
                    StatusName = "En progreso",
                    IsDeleted = false,
                    Employee = obj.UserId.ToString(),
                    TotalOfPieces = obj.TotalOfPieces,
                    UnitPiece = obj.UnitPiece,
                    Contact = obj.Contact,
                    UserId = obj.UserId,
                    OrderDate = obj.OrderDate,
                    DeliveryDate = obj.DeliveryDate,
                    SelectedClient = obj.SelectedClient,
                    SelectedTruck = obj.SelectedTruck,
                    Comment = string.Empty

            };
                _context.Add(newTrack);
                _context.SaveChanges();
            }

            return "Ok";
        }

        public List<AuxTrackModel> GetFilterDataForExcel(ExcelFilters obj)
        {
            if (obj.StartDate.HasValue && obj.EndDate.HasValue)
            {
                var trackList = _context.Track
                    .Where(u => !u.IsDeleted &&
                                 u.DeliveryDate.Date >= obj.StartDate.Value.Date &&
                                 u.DeliveryDate.Date <= obj.EndDate.Value.Date)
                    .OrderByDescending(u => u.DeliveryDate)
                    .Select(i => new AuxTrackModel
                    {
                        TrackCode = i.TrackCode,
                        IdentificationType = i.IdentificationType,
                        DocumentInfo = i.DocumentInfo,
                        IdentificationInfo = i.IdentificationInfo,
                        Telephone = i.Telephone,
                        DeliveryDate= i.DeliveryDate,
                        SelectedCategory = i.SelectedCategory,
                        DeliveryType = i.DeliveryType,
                        TotalWeight = i.TotalWeight,
                        CreatedBy = i.CreatedBy,
                        CreatedOn = i.CreatedOn,
                        StatusOrder = i.StatusOrder,
                        StatusName = i.StatusName,
                        Employee = i.Employee,
                        TotalOfPieces = i.TotalOfPieces,
                        UnitPiece = i.UnitPiece,
                        Contact = i.Contact,
                        UserId = i.UserId,
                        SelectedMeasures = JsonConvert.DeserializeObject<List<string>>(i.SelectedMeasures),
                        MeasureQuantities = JsonConvert.DeserializeObject<List<decimal>>(i.MeasureQuantities)
                    })
                    .ToList();

                return trackList;
            }

            return new List<AuxTrackModel>();
        }
        public AuxTrackModel GetTrackById(Guid Id)
        {
            Tracks? i = _context.Track
                .Where(u => u.IsDeleted != true && u.Id == Id)
                ?.FirstOrDefault();

            var userIdToPrefixMap = _context.AspNetUsers
                .ToDictionary(u => u?.Id, u => new { Prefix = u.Prefix, Name = u.Name, FirstLastName = u.FirstLastName });

            AuxTrackModel trackById = new()
            {
                Id = i.Id,
                TrackCode = "PD-" + userIdToPrefixMap[i.UserId].Prefix + i.TrackCode.PadLeft(7, '0'),
                IdentificationType = i.IdentificationType,
                DocumentInfo = i.DocumentInfo,
                IdentificationInfo = i.IdentificationInfo,
                Telephone = i.Telephone,
                SelectedCategory = i.SelectedCategory,
                SelectedMeasures = JsonConvert.DeserializeObject<List<string>>(i.SelectedMeasures),
                MeasureQuantities = JsonConvert.DeserializeObject<List<decimal>>(i.MeasureQuantities),
                DeliveryType = i.DeliveryType,
                TotalWeight = i.TotalWeight,
                CreatedBy = i.CreatedBy,
                CreatedOn = i.CreatedOn,
                LastUpdatedBy = i.LastUpdatedBy,
                LastUpdatedOn = i.LastUpdatedOn,
                StatusOrder = i.StatusOrder,
                StatusName = i.StatusName,
                TotalOfPieces = i.TotalOfPieces,
                UnitPiece = i.UnitPiece,
                Contact = i.Contact,
                UserId = i.UserId,
                Comment = i.Comment,
                Employee = i.Employee,
                DeliveryDate = i.DeliveryDate,
                OrderDate = i.OrderDate,
                SelectedClient = i.SelectedClient,
                SelectedTruck  = i.SelectedTruck
            };


            return trackById;
        }
        public void CreateCustomer(TrackModel obj)
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
        public void DuplicateTrack(Guid TrackId, Guid userId)
        {
            var userInfo = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);
            var obj = _context.Track.FirstOrDefault(x => x.Id == TrackId);
            if (obj != null && userInfo != null)
            {
                TimeZoneInfo peruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

                DateTime currentTimePeru = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, peruTimeZone);
                var user = _context.AspNetUsers.SingleOrDefault(x => x.Email == userInfo.Email);

                string latestTrackNumber = _context.Track
                .OrderByDescending(i => i.CreatedOn)
                .Select(i => i.TrackCode)
                .FirstOrDefault() ?? string.Empty;
                int nextTrackNumber = 1;
                if (!string.IsNullOrEmpty(latestTrackNumber))
                {
                    nextTrackNumber = int.Parse(latestTrackNumber) + 1;
                }
                if (user != null)
                {
                    Tracks newTrack = new Tracks
                    {
                        Id = Guid.NewGuid(),
                        TrackCode = nextTrackNumber.ToString(),
                        IdentificationType = obj.IdentificationType,
                        DocumentInfo = obj.DocumentInfo,
                        IdentificationInfo = obj.IdentificationInfo,
                        Telephone = obj.Telephone,
                        SelectedCategory = obj.SelectedCategory,
                        SelectedMeasures = obj.SelectedMeasures,
                        MeasureQuantities = obj.MeasureQuantities,
                        DeliveryType = obj.DeliveryType,
                        CreatedBy = userInfo.Email,
                        CreatedOn = currentTimePeru,
                        LastUpdatedBy = userInfo.Email,
                        LastUpdatedOn = currentTimePeru,
                        StatusOrder = 1,
                        StatusName = "En progreso",
                        IsDeleted = false,
                        Employee = userId.ToString(),
                        TotalOfPieces = obj.TotalOfPieces,
                        UnitPiece = obj.UnitPiece,
                        Contact = obj.Contact,
                        UserId = userId,
                        Comment = string.Empty,
                        DeliveryDate =  obj.DeliveryDate,
                        OrderDate = obj.OrderDate,
                        SelectedClient = obj.SelectedClient,
                        SelectedTruck = obj.SelectedTruck,
                        TotalWeight = obj.TotalWeight
                    };
                    _context.Add(newTrack);
                    _context.SaveChanges();
                }
            }

        }
        
        public string UpdateTrack(Guid TrackId, TrackModel obj)
        {
            Tracks? track = _context.Track.Where(x => x.IsDeleted != true && x.Id == TrackId)?.FirstOrDefault();
            TimeZoneInfo peruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

            DateTime currentTimePeru = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, peruTimeZone);
            if (track != null)
            {
                track.IdentificationType = obj.IdentificationType;
                track.DocumentInfo = obj.DocumentInfo;
                track.IdentificationInfo = obj.IdentificationInfo;
                track.Telephone = obj.Telephone;
                track.SelectedCategory = obj.SelectedCategory;
                track.SelectedMeasures = JsonConvert.SerializeObject(obj.SelectedMeasures);
                track.MeasureQuantities = JsonConvert.SerializeObject(obj.MeasureQuantities);
                track.DeliveryType = obj.DeliveryType;                
                track.LastUpdatedBy = obj.CreatedBy;
                track.LastUpdatedOn = currentTimePeru;                
                track.UnitPiece = obj.UnitPiece;
                track.TotalOfPieces = obj.TotalOfPieces;
                track.Contact = obj.Contact;
                track.DeliveryType = obj.DeliveryType;
                track.OrderDate = obj.OrderDate;
                track.DeliveryDate = obj.DeliveryDate;
                track.Comment = string.Empty;

                _context.SaveChanges();
            }

            return "Ok";
        }
        public string RemoveTrack(Guid TrackId)
        {
            Tracks? track = _context.Track.Where(x => x.IsDeleted != true && x.Id == TrackId)?.FirstOrDefault();

            if (track != null)
            {
                track.IsDeleted = true;
                _context.SaveChanges();
            }
            else
            {
                return "Error";
            }

            return "Ok";
        }
        public string UpdateStatusTrack(Guid TrackId, int OrderStatus)
        {
            Tracks? track = _context.Track.Where(x => x.IsDeleted != true && x.Id == TrackId)?.FirstOrDefault();

            if (track != null)
            {
                if (OrderStatus == 2)
                {
                    track.StatusOrder = 2;
                    track.StatusName = "Atendido";
                }
                if (OrderStatus == 3)
                {
                    track.StatusOrder = 3;
                    track.StatusName = "Rechazada";
                }
                _context.SaveChanges();
            }
            else
            {
                return "Error";
            }

            return "Ok";
        }

        
        public string GetCommentById(Guid Id)
        {
            var comment = _context.Track.FirstOrDefault(x => x.Id == Id)?.Comment;
            if (comment == null)
            {
                return string.Empty;
            }

            return comment;
        }
        public void UpdateCommentbyId(Guid Id, string newComment)
        {
            var track = _context.Track.FirstOrDefault(x => x.Id == Id);
            if (track != null)
            {
                track.Comment = newComment;
                _context.SaveChanges();
            }

        }

    }
}
