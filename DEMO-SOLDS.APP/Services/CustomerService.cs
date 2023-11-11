using DEMO_SOLDS.APP.Models;
using R2.DEMO.APP.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public List<Customers> GetAllCustomers()
        {
            var customers = _context.Customers
                .Where(u => u.IsDeleted != true)
                .ToList();

            return customers;
        }

        public Customers CreateCustomer(string? identificationType, string? identificationInfo, string? customerName)
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
