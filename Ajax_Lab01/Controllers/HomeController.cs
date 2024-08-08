using Ajax_Lab01.Models;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace Ajax_Lab01.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private MyDBContext _dbContext;
        public HomeController(ILogger<HomeController> logger,MyDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet("/Home/SiteId/{City?}")] //
        public IActionResult SiteId(string? City)
        {
            //var c = _dbContext.Addresses.Where(Address => Address.SiteId.Contains(City)).Select(Address => Address.SiteId).Distinct().ToList();
            var c = _dbContext.Addresses
                .Where(Address => Address.SiteId.Contains(City))
                .Select(Address => Address.SiteId)
                .Distinct()
                .ToList();
            if(!c.Any()) 
            {
                return NotFound("查無相符的城市，故無法顯示其鄉鎮區");
            }

            return Json(c);
        }

        public IActionResult Index()
        {
            var c = _dbContext.Addresses;
            return View(c);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult homework()
        {
            return View();
        }
        [ResponseCache(Duration = 0,Location = ResponseCacheLocation.None,NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
