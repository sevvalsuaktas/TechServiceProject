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

            if (string.IsNullOrEmpty(user.Password) || user.Password.Length < 6 || user.Password.Length > 10 || !user.Password.All(char.IsDigit))
            {
                ViewBag.ErrorMessage = "Şifre 6 ile 10 karakter arasında olmalı ve SADECE RAKAMLARDAN oluşmalıdır!";
                return View(user); 
            }

            // Yeni personeli veritabanına ekliyoruz
            _context.Users.Add(user);
            _context.SaveChanges();

            // Kayıt başarılıysa arıza listesi ana sayfasına yönlendiriyoruz
            return RedirectToAction("Index", "Incident");
        }

        // personelleri listeleme sayfası
        [HttpGet]
        public IActionResult Index()
        {
            // Sisteme giriş yapmış olan kişinin (Adminin) kullanıcı adını alıyoruz
            var currentUserName = User.Identity.Name;

            // Veritabanından kullanıcıları çekerken kendisi (currentUserName) HARİÇ olanları getiriyoruz
            var users = _context.Users.Where(u => u.Username != currentUserName).ToList();

            // Hangi personelde hangi cihaz var bulabilmek için arıza listesini de sayfaya gönderiyoruz
            ViewBag.AllIncidents = _context.DeviceIncidents.ToList();

            return View(users);
        }

        // personel güncelleme ekranını açma
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // personel güncelleme işlemini veritabanına kaydetme
        [HttpPost]
        public IActionResult Edit(AppUser updatedUser)
        {
            var user = _context.Users.Find(updatedUser.Id);
            if (user != null)
            {
                // Seçtiğimiz yeni kullanıcı adı başkası tarafından kullanılıyor mu? (Kendi ID'miz hariç)
                var exists = _context.Users.Any(x => x.Username == updatedUser.Username && x.Id != updatedUser.Id);
                if (exists)
                {
                    ViewBag.ErrorMessage = "Bu kullanıcı adı zaten başka bir personel tarafından kullanılıyor!";
                    return View(updatedUser);
                }

                // Bilgileri Güncelle
                user.Username = updatedUser.Username;
                user.Password = updatedUser.Password;
                user.Role = updatedUser.Role;

                if (string.IsNullOrEmpty(user.Password) || user.Password.Length < 6 || user.Password.Length > 10 || !user.Password.All(char.IsDigit))
                {
                    ViewBag.ErrorMessage = "Şifre 6 ile 10 karakter arasında olmalı ve SADECE RAKAMLARDAN oluşmalıdır!";
                    return View(updatedUser); 
                }

                _context.SaveChanges();
                return RedirectToAction("Index"); // Başarılıysa listeye geri dön
            }
            return View(updatedUser);
        }

        // personel silme işlemi
        [HttpGet]
        public IActionResult Delete(int id)
        {
            // Önce silinecek personeli buluyoruz
            var user = _context.Users.Find(id);

            if (user != null)
            {
                // Bu personelin üzerinde iş varsa, hata almamak için o işleri boşa çıkarıyoruz (null yapıyoruz)
                var userTasks = _context.DeviceIncidents.Where(x => x.AssignedUser != null && x.AssignedUser.Id == id).ToList();
                foreach (var task in userTasks)
                {
                    task.AssignedUser = null;
                }

                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}