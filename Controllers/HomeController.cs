using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMercado.Models;

namespace MiniMercado.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        
        public IActionResult Index()
        {
            Console.WriteLine("Rol: " + User.FindFirst(ClaimTypes.Role)?.Value);
            return View();
        }

        [Authorize(Roles = "Administrador")] 
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            var storedUsername = _config["AdminUser:Username"];
            var storedHash = _config["AdminUser:PasswordHash"];

            if (username == storedUsername && VerifyPassword(password, storedHash))
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Administrador")
                };

                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Cookies", principal);

                HttpContext.Session.SetString("nombre", username);
                HttpContext.Session.SetString("Rol", "Administrador");

                return RedirectToAction("Index", "MainMenu");
            }

            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            return View();
        }


        [AllowAnonymous]
        private bool VerifyPassword(string password, string storedHash)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            string hashBase64 = Convert.ToBase64String(bytes);
            return hashBase64 == storedHash;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return Content("No tenés permisos para acceder a esta página.");
        }



    }
}
