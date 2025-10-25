using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrivacyConfirmed.Models;

namespace PrivacyConfirmed.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult WhyChoose()
        {
            return View();
        }

        public IActionResult Technical()
        {
            return View();
        }

        public IActionResult AdminConsole()
        {
            return View();
        }

        public IActionResult Pricing()
        {
            return View();
        }

        public IActionResult ComparisonMatrix()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CoreFeatures()
        {
            return View();
        }

        public IActionResult LegalAndCompliance()
        {
            return View();
        }

        public IActionResult HowItHelps()
        {
            return View();
        }

        public IActionResult ReqADemo()
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
