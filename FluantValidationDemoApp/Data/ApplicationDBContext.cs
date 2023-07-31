using FluantValidationDemoApp.Entites;
using Microsoft.EntityFrameworkCore;

namespace FluantValidationDemoApp.Data
{
    public class ApplicationDBContext : DbContext
    {       

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }

    }
}
