using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using OutfitUser = Dress_Up.Models.OutfitUser;

namespace Dress_Up.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AchievementService _achievementService;

        public UserController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            AchievementService achievementService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _achievementService = achievementService;
        }

        // ----------------------------------------------------
        //                   PROFIL / INDEX
        // ----------------------------------------------------
        public async Task<IActionResult> Index(string id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null) return NotFound();

            // Mesaje alerta necitite pentru profilul vizitat
            var messages = _context.AlertMessages
                                   .Where(m => m.UserId == user.Id && !m.IsRead)
                                   .OrderByDescending(m => m.SentAt)
                                   .ToList();

            ViewBag.AlertMessages = messages;

            // Dacă e Admin, trimite toți userii în ViewBag
            if (User.IsInRole("Admin"))
            {
                ViewBag.AllUsers = _context.Users.ToList();
            }

            // Afișare realizări user curent (dacă vizitează propriul profil)
            var achievements = await _achievementService.GetUserAchievementsAsync(currentUserId);
            ViewBag.UserAchievements = achievements;

            // Ce outfit-uri afișăm?
            IQueryable<Outfit> outfitsQuery;
            bool selfOrAdmin = currentUserId == id || User.IsInRole("Admin");

            if (selfOrAdmin)
            {
                outfitsQuery = _context.Outfits.Where(o => o.User.Id == id);
                ViewBag.AfisareButoane = true;
            }
            else
            {
                outfitsQuery = _context.Outfits.Where(o => o.User.Id == id && o.IsPublic);
                ViewBag.AfisareButoane = false;
            }

            ViewBag.UserOutfits = await outfitsQuery.ToListAsync();

            // Outfits salvate (doar proprietar)
            if (selfOrAdmin)
            {
                var saved = _context.OutfitUsers.Where(ou => ou.UserId == id).ToList();
                ViewBag.SavedUOutfits = saved;
                ViewBag.AllOutfits = _context.Outfits.ToList();
            }

            // Mesaje flash
            if (TempData.ContainsKey("message"))
                ViewBag.Msg = TempData["message"]!.ToString();

            ViewBag.Admin = User.IsInRole("Admin");
            return View(user);
        }

        // ----------------------------------------------------
        //                PUBLICĂ UN OUTFIT
        // ----------------------------------------------------
        public IActionResult PublicaOutfit(int id)
        {
            var outfit = _context.Outfits.Include(o => o.User).FirstOrDefault(o => o.Id == id);
            if (outfit == null)
            {
                TempData["message"] = "Outfit-ul nu a fost găsit!";
                return RedirectToAction("Index", new { id = _userManager.GetUserId(User) });
            }

            outfit.IsPublic = true;
            _context.SaveChanges();

            TempData["message"] = "Outfit-ul a fost publicat cu succes!";
            return RedirectToAction("Index", new { id = outfit.User.Id });
        }

        // ----------------------------------------------------
        //                EDITARE PROFIL
        // ----------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            return user == null ? NotFound() : View(user);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.About = model.About;
            user.ProfilePictureUrl = model.ProfilePictureUrl;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return RedirectToAction("Index", new { id = user.Id });

            foreach (var err in result.Errors) ModelState.AddModelError(string.Empty, err.Description);
            return View(model);
        }

        // ----------------------------------------------------
        //    ADMIN - add/remove role
        // ----------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> MakeAdmin(string id)
            => await ToggleAdmin(id, add: true);

        [HttpPost]
        public async Task<IActionResult> RmvAdmin(string id)
            => await ToggleAdmin(id, add: false);

        private async Task<IActionResult> ToggleAdmin(string id, bool add)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Utilizatorul nu a fost găsit.";
                return RedirectToAction("Index");
            }

            IdentityResult result = add
                ? await _userManager.AddToRoleAsync(user, "Admin")
                : await _userManager.RemoveFromRoleAsync(user, "Admin");

            if (!result.Succeeded)
            {
                TempData["Error"] = add
                    ? "Nu s-a putut adăuga rolul de admin."
                    : "Nu s-a putut șterge rolul de admin.";
                return RedirectToAction("Index", new { id });
            }

            TempData["Success"] = add
                ? "Utilizatorul a devenit admin!"
                : "Utilizatorul nu mai are rol de admin!";

            if (!add && user.Id == _userManager.GetUserId(User))
                await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction("Index", new { id });
        }

        // ----------------------------------------------------
        //            SALVEAZĂ / UNSAVE OUTFIT
        // ----------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveOutfit(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            if (await _context.OutfitUsers.AnyAsync(ou => ou.UserId == userId && ou.OutfitId == id))
            {
                TempData["message"] = "Ai salvat deja acest outfit!";
                return RedirectToAction("Index", new { id = userId });
            }

            _context.OutfitUsers.Add(new OutfitUser { UserId = userId, OutfitId = id });
            await _context.SaveChangesAsync();

            TempData["message"] = "Outfit salvat!";
            return RedirectToAction("Index", new { id = userId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UnsaveOutfit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var link = await _context.OutfitUsers
                                       .FirstOrDefaultAsync(ou => ou.UserId == userId && ou.OutfitId == id);

            if (link != null)
            {
                _context.OutfitUsers.Remove(link);
                await _context.SaveChangesAsync();
                TempData["message"] = "Outfit-ul a fost eliminat din lista ta.";
            }
            else
            {
                TempData["message"] = "Outfit-ul nu se găsește în lista ta.";
            }

            return RedirectToAction("Index", new { id = userId });
        }
    }
}
