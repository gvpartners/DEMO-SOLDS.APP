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
    public class VisitService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;


        public VisitService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public VisitListResponse GetAllVisits(VisitPage pag)
        {
            int recordsToSkip = (pag.PageNumber) * pag.PageSize;

            var userIdToPrefixMap = _context.AspNetUsers
                .ToDictionary(u => u.Id, u => new { Prefix = u.Prefix, Name = u.Name, FirstLastName = u.FirstLastName });
            var query = _context.Visit.Where(i => i.IsDeleted != true);

            if (pag.Filters != null)
            {
                if (!string.IsNullOrEmpty(pag.Filters.VisitCodeFilter))
                {
                    query = query.Where(i => i.VisitCode.Contains(pag.Filters.VisitCodeFilter));
                }
                if (!string.IsNullOrEmpty(pag.Filters.ClientFilter))
                {
                    query = query.Where(i => i.Client.Contains(pag.Filters.ClientFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.WorkFilter))
                {
                    query = query.Where(i => i.Work.Contains(pag.Filters.WorkFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.WorkAddressFilter))
                {
                    query = query.Where(i => i.WorkAddress.Contains(pag.Filters.WorkAddressFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.ContactsFilter))
                {
                    query = query.Where(i => i.Contacts.Contains(pag.Filters.ContactsFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.VisitReasonFilter))
                {
                    query = query.Where(i => i.VisitReason.Contains(pag.Filters.VisitReasonFilter));
                }

                if (!string.IsNullOrEmpty(pag.Filters.CreatedByFilter))
                {
                    query = query.Where(i => i.CreatedBy.Contains(pag.Filters.CreatedByFilter));
                }
                if (!string.IsNullOrEmpty(pag.Filters.StatusNameFilter))
                {
                    query = query.Where(i => i.StatusName.Contains(pag.Filters.StatusNameFilter));
                }
                if (pag.Filters.CreatedOnFilter.HasValue)
                {
                    query = query.Where(i => i.CreatedOn.Date == pag.Filters.CreatedOnFilter.Value.Date);
                }

                
            }
            query = query.OrderByDescending(i => i.CreatedOn);
            var totalOfVisits = query.Count();
            var visitsList = query
                .Skip(recordsToSkip)
                .Take(pag.PageSize)
                .Select(i => new AuxVisitModel
                {
                    
                    Id = i.Id,
                    VisitCode = "VT-" + userIdToPrefixMap[Guid.Parse(i.CreatedBy)].Prefix + i.VisitCode.PadLeft(7, '0'),
                    Client = i.Client,
                    Work = i.Work,
                    Contacts = i.Contacts,
                    CreatedBy = userIdToPrefixMap[Guid.Parse(i.CreatedBy)].Name + " " + userIdToPrefixMap[Guid.Parse(i.CreatedBy)].FirstLastName,
                    VisitReason = i.VisitReason,
                    WorkAddress = i.WorkAddress,
                    CreatedOn = i.CreatedOn,
                    StatusName = i.StatusName,
                    StatusOrder = i.StatusOrder
                })
                .ToList();

            return new VisitListResponse
            {
                Total = totalOfVisits,
                Visits = visitsList
            };
        }
        
        public string UpdateVisitStatus(Guid visitId, int statusOrder)
        {
            Visit visit = _context.Visit.FirstOrDefault(x => x.Id == visitId);
            if (visit != null)
            {
                visit.StatusName = "Atendido";
                visit.StatusOrder = statusOrder;
                _context.SaveChanges();
            }
            return "ok";
        }
        public void UpdateCommentbyId(Guid Id, string newComment)
        {
            var visit = _context.Visit.FirstOrDefault(x => x.Id == Id);
            if (visit != null)
            {
                visit.Comment = newComment;
                _context.SaveChanges();
            }

        }

        public string GetCommentById(Guid Id)
        {
            var comment = _context.Visit.FirstOrDefault(x => x.Id == Id)?.Comment;
            if (comment == null)
            {
                return string.Empty;
            }

            return comment;
        }
        public string RemoveVisit(Guid VisitId)
        {
            Visit? visit = _context.Visit.Where(x => x.IsDeleted != true && x.Id == VisitId)?.FirstOrDefault();

            if (visit != null)
            {
                visit.IsDeleted = true;
                _context.SaveChanges();
            }
            else
            {
                return "Error";
            }

            return "Ok";
        }
        public string UpdateVisit(Guid visitId, Visit updatedVisit)
        {
            // Buscar la visita existente en la base de datos
            Visit? visit = _context.Visit
                .Where(x => x.IsDeleted != true && x.Id == visitId)
                .FirstOrDefault();

            if (visit == null)
            {
                return "Error: Visita no encontrada.";
            }

            try
            {
                // Actualizar los campos de la visita con los nuevos valores
                visit.Client = updatedVisit.Client;
                visit.Work = updatedVisit.Work;
                visit.WorkAddress = updatedVisit.WorkAddress;
                visit.Contacts = updatedVisit.Contacts; // Asegúrate de que Contacts esté en el modelo
                visit.VisitReason = updatedVisit.VisitReason;

                // Guardar los cambios en la base de datos
                _context.SaveChanges();

                return "Ok: Visita actualizada correctamente.";
            }
            catch (Exception ex)
            {
                // Manejar errores
                return $"Error: No se pudo actualizar la visita. Detalles: {ex.Message}";
            }
        }
    }
}
