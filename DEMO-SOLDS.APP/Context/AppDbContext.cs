using DEMO_SOLDS.APP.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace R2.DEMO.APP.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Users> AspNetUsers { get; set; }
        public DbSet<Invoices> Invoices { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Tracks> Track { get; set; }
        public DbSet<Visit> Visit { get; set; }
    }
}
