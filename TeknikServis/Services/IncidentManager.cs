using TechService.DataAccess;
using TechService.Models.Entities;
using Microsoft.EntityFrameworkCore;

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
            // Include metodu ile Arıza kayıtlarını çekerken, ona atanmış Personel (User) bilgisini de tabloya dahil (Join) ediyoruz
            return _context.DeviceIncidents
                           .Include(x => x.AssignedUser)
                           .OrderByDescending(x => x.CreatedDate)
                           .AsNoTracking().ToList(); 
        }

        public void DeleteIncident(int id)
        {
            var incident = _context.DeviceIncidents.Find(id);
            if (incident != null)
            {
                _context.DeviceIncidents.Remove(incident);
                _context.SaveChanges();
            }
        }

        public DeviceIncident GetIncidentById(int id)
        {
            return _context.DeviceIncidents.Find(id);
        }

        public void UpdateIncident(DeviceIncident incident, IFormFile? imageFile)
        {
            // Veritabanındaki eski kaydı buluyoruz
            var existingValue = _context.DeviceIncidents.Find(incident.Id);

            if (existingValue != null)
            {
                // Temel bilgileri güncelliyoruz
                existingValue.CustomerName = incident.CustomerName;
                existingValue.DeviceModel = incident.DeviceModel;
                existingValue.IssueDescription = incident.IssueDescription;
                existingValue.AssignedUserId = incident.AssignedUserId;
                existingValue.Status = incident.Status;

                // EĞER YENİ BİR RESİM YÜKLENDİYSE GÜNCELLE
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        imageFile.CopyTo(memoryStream);
                        existingValue.DeviceImage = memoryStream.ToArray();
                    }
                }
                // Resim yüklenmediyse eski resim veritabanında korunur

                _context.SaveChanges();
            }
        }
    }
}
