using System.Diagnostics;
using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Dress_Up.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        // singurul constructor
        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// Feed pentru guests – afişează outfit‑urile publice
        public IActionResult HomeGuest()
        {
            var outfits = _context.Outfits
                                      .Include(o => o.User)
                                      .Include(o => o.Comments)
                                      .Include(o => o.Votes)        // aducem toate Vote‑urile
                                      .Where(o => o.IsPublic)
                                      .OrderByDescending(o => o.Date_added)
                                      .ToList();

            return View(outfits);
        }


        public IActionResult Index()
        {
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
