using TechService.DataAccess;
using TechService.Models.Entities;

namespace TechService.Services
{
    public class IncidentManager : IIncidentService
    {
        private readonly AppDbContext _context;

        // Dependency Injection ile veritabanı bağlamımızı (Context) çağırıyoruz
        public IncidentManager(AppDbContext context)
        {
            _context = context;
        }

        public void AddIncident(DeviceIncident incident, IFormFile imageFile)
        {
            // Eğer kullanıcı bir resim yüklediyse, onu byte[] dizisine çevir
            if (imageFile != null && imageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    imageFile.CopyTo(memoryStream);
                    incident.DeviceImage = memoryStream.ToArray();
                }
            }

            // Veritabanına ekle ve kaydet
            _context.DeviceIncidents.Add(incident);
            _context.SaveChanges();
        }

        public List<DeviceIncident> GetAllIncidents()
        {
            // Kayıtları en yeniden en eskiye doğru sıralayarak getiriyoruz
            return _context.DeviceIncidents.OrderByDescending(x => x.CreatedDate).ToList();
        }
    }
}
