using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniMercado.Models;
using Microsoft.AspNetCore.Authorization;

namespace MiniMercado.Controllers
{
    [Authorize(Roles = "Administrador")] 
    public class MainMenuController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public MainMenuController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
