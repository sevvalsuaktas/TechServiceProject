using Microsoft.AspNetCore.Mvc;
using TechService.Models.Entities;
using TechService.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TechService.DataAccess;

namespace TechService.Controllers
{
    [Authorize]
    public class IncidentController : Controller
    {
        private readonly IIncidentService _incidentService;
        private readonly AppDbContext _context;

        // Servisimizi Dependency Injection ile Controller'a alıyoruz
        public IncidentController(IIncidentService incidentService, AppDbContext context)
        {
            _incidentService = incidentService;
            _context = context;
        }

        // listeleme ve arama işlemi
        public IActionResult Index(string searchString)
        {
            // Giriş yapan kişinin Rolünü ve ID'sini okuyoruz
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var values = _incidentService.GetAllIncidents();

            // EĞER GİRİŞ YAPAN ÇALIŞAN İSE: Sadece AssignedUserId'si kendi ID'sine eşit olanları filtrele
            if (userRole == "Employee")
            {
                values = values.Where(x => x.AssignedUserId == userId).ToList();
            }

            // Arama filtrelemesi (Hem admin hem çalışan arama yapabilsin)
            if (!string.IsNullOrEmpty(searchString))
            {
                values = values.Where(x =>
                    x.CustomerName.ToLower().Contains(searchString.ToLower()) ||
                    x.DeviceModel.ToLower().Contains(searchString.ToLower())).ToList();
            }

            return View(values); ;
        }

        // Yeni Kayıt Ekleme Sayfasını Açma
        [HttpGet]
        public IActionResult Create() 
        {
            // Veritabanından sadece rolü "Employee" olanları alıp ViewBag ile arayüze taşıyoruz
            ViewBag.Employees = _context.Users.Where(x => x.Role == "Employee").ToList();
            return View();
        }

        // Yeni Kayıt Formu Gönderildiğinde Çalışacak Metot
        [HttpPost]
        public IActionResult Create(DeviceIncident incident, IFormFile imageFile)
        {
            _incidentService.AddIncident(incident, imageFile);
            return RedirectToAction("Index"); // Kayıt bitince listeye geri dön
        }

        // SADECE ADMİN SİLEBİLİR
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            _incidentService.DeleteIncident(id);
            // Silme işlemleri (Servis çağrısı)
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var value = _incidentService.GetIncidentById(id);

            // Atama listesini dropdown için tekrar dolduruyoruz
            ViewBag.Employees = _context.Users.Where(x => x.Role == "Employee").ToList();

            return View(value);
        }

        [HttpPost]
        public IActionResult Edit(DeviceIncident incident, IFormFile? imageFile)
        {
            _incidentService.UpdateIncident(incident, imageFile);
            return RedirectToAction("Index");
        }
    }
}
