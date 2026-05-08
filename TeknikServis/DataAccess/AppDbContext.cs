using Microsoft.EntityFrameworkCore;
using TechService.Models.Entities;

namespace TechService.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DeviceIncident> DeviceIncidents { get; set; }
        public DbSet<AppUser> Users { get; set; }
    }
}