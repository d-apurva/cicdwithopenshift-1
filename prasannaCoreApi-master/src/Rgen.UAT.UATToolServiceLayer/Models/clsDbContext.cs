using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.IO;

namespace Rgen.UAT.UATToolServiceLayer.Models
{
    public class clsDbContext : DbContext
    {
        public clsDbContext(DbContextOptions<clsDbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var connectionStringConfig = builder.Build();

            optionsBuilder.UseSqlServer(connectionStringConfig.GetConnectionString("UatContainerConnectionString"));
        }

      



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ForSqlServerUseIdentityColumns();
        }
    }
}
