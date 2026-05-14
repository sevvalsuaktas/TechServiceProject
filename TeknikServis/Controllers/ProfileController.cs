using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechService.DataAccess;
using TechService.Models.Entities;

namespace TechService.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Edit()
        {
            // Login olurken sisteme kaydettiğimiz giriş biletinden (Claim) kullanıcının kendi ID'sini çekiyoruz
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Index", "Login");

            int userId = int.Parse(userIdStr);
            var user = _context.Users.Find(userId);

            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(AppUser p)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdStr);

            var user = _context.Users.Find(userId);
            if (user != null)
            {
                // Yeni seçtiği kullanıcı adı başka birisinde var mı? (Kendi ID'si hariç)
                var exists = _context.Users.Any(x => x.Username == p.Username && x.Id != userId);
                if (exists)
                {
                    ViewBag.ErrorMessage = "Bu kullanıcı adı zaten başka biri tarafından kullanılıyor!";
                    return View(p);
                }

                if (string.IsNullOrEmpty(p.Password) || p.Password.Length < 6 || p.Password.Length > 10 || !p.Password.All(char.IsDigit))
                {
                    ViewBag.ErrorMessage = "Şifre 6 ile 10 karakter arasında olmalı ve SADECE RAKAMLARDAN oluşmalıdır!";
                    return View(p);
                }

                // Bilgileri güncelle
                user.Username = p.Username;
                user.Password = p.Password;
                _context.SaveChanges();

                // Bilgiler değiştiği için tarayıcıdaki eski çerezin (Cookie) yenilenmesi lazım.
                // Bu yüzden kullanıcıyı çıkış yapma (Logout) metoduna yönlendiriyoruz.
                return RedirectToAction("Logout", "Login");
            }

            return View(p);
        }
    }
}