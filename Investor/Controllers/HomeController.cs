using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Investor.Models;
using Microsoft.AspNetCore.Http;

namespace Investor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(bool logout = false)
        {
            //For testing purposes
            HttpContext.Session.SetString("Username", "SparkyMelon");
            HttpContext.Session.SetInt32("Id", 1);

            if (logout)
            {
                HttpContext.Session.SetString("Username", "");
                HttpContext.Session.SetInt32("Id", -1);
            }

            return View();
        }

        public IActionResult Privacy()
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
