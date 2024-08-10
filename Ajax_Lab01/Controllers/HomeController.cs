using Ajax_Lab01.Models;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using System.Text;

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
            var c = _dbContext.Addresses
            .Where(Address => Address.SiteId.Contains(City))
            .Select(Address => Address.SiteId)
            .Distinct()
            .ToList();

            if(!c.Any()) {
                return NotFound();
            }
            return Json(c);
        }


        [HttpGet("/Home/Road/{SiteId?}")] //
        public IActionResult Road(string? SiteId)
        {
            //var c = _dbContext.Addresses.Where(Address => Address.SiteId.Contains(City)).Select(Address => Address.SiteId).Distinct().ToList();
            var c = _dbContext.Addresses
                .Where(Address => Address.SiteId.Contains(SiteId))
                .Select(Address => Address.Road)
                .Distinct()
                .ToList();
            if(!c.Any()) {
                return NoContent();
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
