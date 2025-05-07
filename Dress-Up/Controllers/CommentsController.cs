//using Dress_Up.Data;
//using Dress_Up.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;

//namespace Dress_Up.Controllers
//{
//    public class CommentsController : Controller
//    {
//        private readonly ApplicationDbContext db;

//        private readonly UserManager<User> _userManager;

//        private readonly RoleManager<IdentityRole> _roleManager;

//        public CommentsController(
//            ApplicationDbContext context,
//            UserManager<User> userManager,
//            RoleManager<IdentityRole> roleManager
//            )
//        {
//            db = context;

//            _userManager = userManager;

//            _roleManager = roleManager;
//        }

//        [HttpPost]
//        [Authorize(Roles = "User,Admin")]
//        public IActionResult Delete(int id)
//        {
//            Comment comm = db.Comments.Find(id);

//            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
//            {
//                db.Comments.Remove(comm);
//                db.SaveChanges();
//                return Redirect("/Bookmarks/Show/" + comm.BookmarkId);
//            }

//            else
//            {
//                TempData["message"] = "You do not have the right to delete the commen";
//                TempData["messageType"] = "alert-danger";
//                return RedirectToAction("Index", "Bookmarks");
//            }
//        }

//        [Authorize(Roles = "User,Admin")]
//        public IActionResult Edit(int id)
//        {
//            Comment comm = db.Comments.Find(id);

//            if (comm.UserId == _userManager.GetUserId(User))
//            {
//                return View(comm);
//            }

//            else
//            {
//                TempData["message"] = "You do not have the right to edit the comment";
//                TempData["messageType"] = "alert-danger";
//                return RedirectToAction("Index", "Bookmarks");
//            }
//        }

//        [HttpPost]
//        [Authorize(Roles = "User,Admin")]
//        public IActionResult Edit(int id, Comment requestComment)
//        {
//            Comment comm = db.Comments.Find(id);

//            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
//            {
//                if (ModelState.IsValid)
//                {
//                    comm.Content = requestComment.Content;

//                    db.SaveChanges();

//                    return Redirect("/Bookmarks/Show/" + comm.BookmarkId);
//                }
//                else
//                {
//                    return View(requestComment);
//                }
//            }
//            else
//            {
//                TempData["message"] = "You do not have the right to make changes";
//                TempData["messageType"] = "alert-danger";
//                return RedirectToAction("Index", "Bookmarks");
//            }
//        }
//        public IActionResult Index()
//        {
//            return View();
//        }
//    }
//}
