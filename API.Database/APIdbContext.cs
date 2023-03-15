using API.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API.Database
{
    public class APIdbContext : DbContext
    {
        public APIdbContext(DbContextOptions<APIdbContext> options) : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=PMS;Trusted_Connection=True; TrustServerCertificate=True");
        //    }
        //}
    }   
}