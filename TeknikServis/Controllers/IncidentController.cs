using Microsoft.AspNetCore.Mvc;
using TechService.Models.Entities;
using TechService.Services;

namespace TechService.Controllers
{
    public class IncidentController : Controller
    {
        private readonly IIncidentService _incidentService;

        // Servisimizi Dependency Injection ile Controller'a alıyoruz
        public IncidentController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        // listeleme ve arama işlemi
        public IActionResult Index(string searchString)
        {
            var values = _incidentService.GetAllIncidents();

            // Eğer arama kutusuna bir şey yazılmışsa, listeyi filtrele
            if (!string.IsNullOrEmpty(searchString))
            {
                values = values.Where(x =>
                    x.CustomerName.ToLower().Contains(searchString.ToLower()) ||
                    x.DeviceModel.ToLower().Contains(searchString.ToLower())).ToList();
            }

            return View(values);
        }

        // Yeni Kayıt Ekleme Sayfasını Açma
        [HttpGet]
        public IActionResult Create() 
        {
            return View();
        }

        // Yeni Kayıt Formu Gönderildiğinde Çalışacak Metot
        [HttpPost]
        public IActionResult Create(DeviceIncident incident, IFormFile imageFile)
        {
            _incidentService.AddIncident(incident, imageFile);
            return RedirectToAction("Index"); // Kayıt bitince listeye geri dön
        }
    }
}
