using System.Diagnostics;
using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Authentication;              //  SignOutAsync
using Microsoft.AspNetCore.Identity;                   //  IdentityConstants
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dress_Up.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult HomeGuest()
        {
            var outfits = _context.Outfits
                                  .Include(o => o.User)
                                  .Include(o => o.Comments)
                                  .Include(o => o.Votes)
                                  .Where(o => o.IsPublic)
                                  .OrderByDescending(o => o.Date_added)
                                  .ToList();

            return View(outfits);
        }

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            //   delogare: șterge cookie‑ul „Identity.Application”
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            //   redirect la pagina principală
            return RedirectToAction("Index", "Home");
        }
    }
}