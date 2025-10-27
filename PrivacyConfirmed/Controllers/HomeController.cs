using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrivacyConfirmed.Models;
using PrivacyConfirmedModel;
using PrivacyConfirmedBAL.Interfaces;

namespace PrivacyConfirmed.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IContactUsService _contactUsService;

        public HomeController(ILogger<HomeController> logger, IContactUsService contactUsService)
        {
            _logger = logger;
            _contactUsService = contactUsService;
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

        #region Contact Us Actions
        
        /// <summary>
        /// GET: Display the Contact Us form
        /// </summary>
        [HttpGet]
        public IActionResult ContactUs()
        {
            return View(new ContactUsModel());
        }

        /// <summary>
        /// POST: Process the Contact Us form submission
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(ContactUsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _contactUsService.SaveContactAsync(model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(ContactUsSuccess));
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form");
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
                return View(model);
            }
        }

        /// <summary>
        /// GET: Display success page after form submission
        /// </summary>
        public IActionResult ContactUsSuccess()
        {
            return View();
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
