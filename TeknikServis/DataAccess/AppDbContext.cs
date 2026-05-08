using Microsoft.EntityFrameworkCore;
using TechService.Models.Entities;

namespace TechService.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DeviceIncident> DeviceIncidents { get; set; }
        // İleride buraya Login için User tablosunu da ekleyeceğiz
    }
}