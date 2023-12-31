﻿using DEMO_SOLDS.APP.Models;
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

        public Customers CreateCustomer(Customers customer)
        {
            var existCustomer = _context.Customers.FirstOrDefault(x => x.IdentificationInfo == customer.IdentificationInfo);
            if (existCustomer == null)
            {
                var newCustomer = new Customers
                {
                    Id = Guid.NewGuid(),
                    CustomerName = customer.CustomerName,
                    IdentificationType = customer.IdentificationType,
                    IdentificationInfo = customer.IdentificationInfo,
                    CustomerAddress = customer.CustomerAddress??"",
                    IsDeleted = false
                };

                _context.Customers.Add(newCustomer);
                _context.SaveChanges();

                return newCustomer;
            }
            
            return new Customers();

        }
        public bool GetIsCustomerInDb(string customerNumber)
        {
            var customer = _context.Customers.FirstOrDefault(x => x.IdentificationInfo == customerNumber);
            return customer != null;
        }
        public string GetCustomerAddress(string documentInfo)
        {
            var customer = _context.Customers.FirstOrDefault(x => x.IdentificationInfo == documentInfo);
            string? customerAddress = string.Empty;
            if(customer != null)
            {
                customerAddress = customer.CustomerAddress;
            }
            return customerAddress;
        }
        public Customers UpdateCustomer(Guid customerId, Customers customer)
        {
            var existingCustomer = _context.Customers.Find(customerId);

            if (existingCustomer != null)
            {
                existingCustomer.CustomerName = customer.CustomerName;
                existingCustomer.IdentificationType = customer.IdentificationType;
                existingCustomer.IdentificationInfo = customer.IdentificationInfo;
                existingCustomer.CustomerAddress = customer.CustomerAddress??"";
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
