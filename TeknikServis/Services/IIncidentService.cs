using TechService.Models.Entities;

namespace TechService.Services
{
    public interface IIncidentService
    {
        List<DeviceIncident> GetAllIncidents(); // tüm kayıtları getirecek metot

        void AddIncident(DeviceIncident incident, IFormFile imageFile); // Yeni kayıt ekleyecek metot (Resim dosyasıyla birlikte)
        void DeleteIncident(int id);
        DeviceIncident GetIncidentById(int id);
        void UpdateIncident(DeviceIncident incident, IFormFile? imageFile);
    }
}
