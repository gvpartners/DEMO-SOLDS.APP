using DEMO_SOLDS.APP.Models;
using R2.DEMO.APP.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using DEMO_SOLDS.APP.Models.Pagination;

namespace DEMO_SOLDS.APP.Services
{
    public class CustomerService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public CustomerService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public CustomerListResponse GetAllCustomers(CustomerPage pag)
        {
            // Calcular el número de registros para omitir
            int recordsToSkip = (pag.PageNumber) * pag.PageSize;

            // Crear la consulta base de clientes (sin filtros aplicados)
            var query = _context.Customers.Where(i => i.IsDeleted != true);

            // Aplicar filtros si están presentes
            if (pag.Filters != null)
            {
                if (!string.IsNullOrEmpty(pag.Filters.IdentificationInfoFilter))
                {
                    query = query.Where(i => i.IdentificationInfo.Contains(pag.Filters.IdentificationInfoFilter));
                }
                if (!string.IsNullOrEmpty(pag.Filters.IdentificationTypeFilter))
                {
                    query = query.Where(i => i.IdentificationType.Contains(pag.Filters.IdentificationTypeFilter));
                }
                if (!string.IsNullOrEmpty(pag.Filters.CustomerNameFilter))
                {
                    query = query.Where(i => i.CustomerName.Contains(pag.Filters.CustomerNameFilter));
                }
            }

            // Contar el total de facturas sin aplicar paginación
            var totalOfCustomers = query.Count();

            var customerList = query.Skip(recordsToSkip).Take(pag.PageSize).ToList();

            // Crear y devolver la respuesta con la lista paginada de clientes y el total de clientes
            return new CustomerListResponse
            {
                Total = totalOfCustomers,
                Customers = customerList
            };
        }

        public Customers CreateCustomer(string? identificationType, string? identificationInfo, string? customerName)
        {
            var existCustomer = _context.Customers.FirstOrDefault(x => x.IdentificationInfo == identificationInfo);
            if (existCustomer == null)
            {
                var newCustomer = new Customers
                {
                    Id = Guid.NewGuid(),
                    CustomerName = customerName,
                    IdentificationType = identificationType,
                    IdentificationInfo = identificationInfo,
                    IsDeleted = false
                };

                _context.Customers.Add(newCustomer);
                _context.SaveChanges();

                return newCustomer;
            }
            
            return new Customers();

        }

        public Customers UpdateCustomer(Guid customerId, string? identificationType, string? identificationInfo, string? customerName)
        {
            var existingCustomer = _context.Customers.Find(customerId);

            if (existingCustomer != null)
            {
                existingCustomer.CustomerName = customerName;
                existingCustomer.IdentificationType = identificationType;
                existingCustomer.IdentificationInfo = identificationInfo;

                _context.SaveChanges();
            }

            return existingCustomer??new Customers();
        }

        public void RemoveCustomer(Guid customerId)
        {
            var existingCustomer = _context.Customers.Find(customerId);

            if (existingCustomer != null)
            {
                existingCustomer.IsDeleted = true;
                _context.SaveChanges();
            }
        }
    }
}
