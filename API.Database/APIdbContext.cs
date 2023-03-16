using API.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Database
{
    public class APIdbContext : DbContext
    {
        public APIdbContext(DbContextOptions<APIdbContext> options) : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }

    }   
}