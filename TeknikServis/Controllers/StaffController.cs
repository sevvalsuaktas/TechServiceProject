using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechService.DataAccess;
using TechService.Models.Entities;

namespace TechService.Controllers
{
    // Bu sayfaya sadece Admin rolündekilerin girmesini sağlıyoruz!
    // Personel linki bilse bile sayfaya erişemez.
    [Authorize(Roles = "Admin")]
    public class StaffController : Controller
    {
        private readonly AppDbContext _context;

        public StaffController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(AppUser user)
        {
            // Kullanıcı adı daha önce alınmış mı diye kontrol ediyoruz
            var exists = _context.Users.Any(x => x.Username == user.Username);

            if (exists)
            {
                ViewBag.ErrorMessage = "Bu kullanıcı adı zaten sistemde kayıtlı!";
                return View(user);
            }

            // Yeni personeli veritabanına ekliyoruz
            _context.Users.Add(user);
            _context.SaveChanges();

            // Kayıt başarılıysa arıza listesi ana sayfasına yönlendiriyoruz
            return RedirectToAction("Index", "Incident");
        }
    }
}