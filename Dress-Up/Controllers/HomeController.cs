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


        public IActionResult HomeGuest(string filter, string searchTerm)
        {
            var query = _context.Outfits
                .Include(o => o.User)
                .Include(o => o.Comments)
                .Include(o => o.Votes)
                .Where(o => o.IsPublic);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var pattern = $"%{searchTerm}%";
                query = query.Where(o =>
                    EF.Functions.Like(o.Name, pattern) ||
                    (o.User != null && EF.Functions.Like(o.User.UserName, pattern)));
            }

            query = filter == "popular"
                ? query.OrderByDescending(o => o.Votes.Count(v => v.EventId == null))
                : query.OrderByDescending(o => o.Date_added);

            ViewBag.SearchTerm = searchTerm;
            return View(query.ToList());
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