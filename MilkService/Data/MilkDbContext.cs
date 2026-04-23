using Microsoft.EntityFrameworkCore;
using MilkService.Models;
using System.Collections.Generic;
namespace MilkService.Data
{   

    public class MilkDbContext : DbContext
    {
        public MilkDbContext(DbContextOptions<MilkDbContext> options)
            : base(options)
        {
        }

        public DbSet<Milk> Milks { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
    }
}
