using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechService.DataAccess;
using TechService.Models.Entities;

namespace TechService.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(AppUser user)
        {
            // Veritabanında kullanıcıyı ara
            var dataValue = _context.Users.FirstOrDefault(x => x.Username == user.Username && x.Password == user.Password);

            // LoginController.cs içindeki başarılı giriş kısmı
            if (dataValue != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, dataValue.Username),
                    new Claim(ClaimTypes.Role, dataValue.Role), // Kullanıcının Rolü
                    new Claim(ClaimTypes.NameIdentifier, dataValue.Id.ToString()) // Kullanıcının ID'si
                };

                var useridentity = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);

                await HttpContext.SignInAsync(principal);
                return RedirectToAction("Index", "Incident");
            }

            // Kullanıcı adı veya şifre hatalıysa mesaj gönder
            ViewBag.ErrorMessage = "Geçersiz kullanıcı adı veya şifre!";
            return View();
        }

        // Çıkış Yapma Metodu
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
