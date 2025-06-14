using Dress_Up.Controllers;
using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;
using System.Collections.Generic;
using System;
using System.Linq;

namespace DressUpTests
{
    public class OutfitTests
    {
        private ApplicationDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unică pt fiecare test
                .Options;
            return new ApplicationDbContext(options);
        }

        private Mock<UserManager<User>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
                store.Object, null, null, null, null, null, null, null, null
            );
        }

        private ClaimsPrincipal GetMockUser(string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        [Fact]
        public void Show_ReturnsOutfitInViewBag()
        {
            var db = GetInMemoryDb();
            var user = new User { Id = "user1", UserName = "Test" };
            var outfit = new Outfit { Id = 1, Name = "Outfit1", User = user };
            db.Outfits.Add(outfit);
            db.SaveChanges();

            var controller = new OutfitController(db, GetMockUserManager().Object);

            var result = controller.Show(1) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Outfit1", ((Outfit)result.ViewData["Outfit"]).Name);
        }

        [Fact]
        public void Edit_Post_ValidData_UpdatesOutfit()
        {
            var db = GetInMemoryDb();
            Outfit outfit = new Outfit();
            outfit.Image = "test.jpg";
            outfit.Name = "old";
            outfit.User = new User() { Id = "u1", UserName = "TestUser" };
            outfit.Date_added = DateTime.Now;
            outfit.IsPublic = false;
            outfit.Description = "oldd";
            db.Outfits.Add(outfit);
            db.SaveChanges();

            var controller = new OutfitController(db, GetMockUserManager().Object);

            var updated = new Outfit { Id = 1, Name = "NewName", Description = "NewDesc", IsPublic = true };
            var result = controller.Edit(1, updated) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("NewName", db.Outfits.Find(1).Name);
        }

        [Fact]
        public void Delete_RemovesOutfitAndRelatedEntities()
        {
            var db = GetInMemoryDb();
            var user = new User { Id = "u1", UserName = "User", Outfits = new List<Outfit>() };
            Outfit outfit = new Outfit();
            outfit.Image = "test.jpg";
            outfit.Name = "Test";
            outfit.User = user;
            outfit.Date_added = DateTime.Now;
            outfit.IsPublic = false;
            outfit.Description = "test";
            user.Outfits.Add(outfit);

            db.Users.Add(user);
            db.Outfits.Add(outfit);
            db.Comments.Add(new Comment { Id = 1, OutfitId = 1, Content = "Test", UserId = "u1" });
            db.Votes.Add(new Vote { Id = 1, OutfitId = 1, UserId = "u1" });
            db.UserEvents.Add(new UserEvent { Id = 1, OutfitId = 1, UserId = "u1" });


            db.SaveChanges();

            var controller = new OutfitController(db, GetMockUserManager().Object);
            var result = controller.Delete(1) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Empty(db.Outfits);
            Assert.Empty(db.Comments);
            Assert.Empty(db.Votes);
            Assert.Empty(db.UserEvents);
        }

        [Fact]
        public void AddComment_Valid_AddsComment()
        {
            var db = GetInMemoryDb();
            Outfit outfit = new Outfit();
            outfit.Image = "test.jpg";
            outfit.Name = "Test";
            outfit.User = new User() { Id = "u1", UserName = "TestUser" };
            outfit.Date_added = DateTime.Now;
            outfit.IsPublic = false;
            outfit.Description = "test";
            db.Outfits.Add(outfit);
            db.SaveChanges();

            var mockUserManager = GetMockUserManager();
            var controller = new OutfitController(db, mockUserManager.Object);

            var mockUser = GetMockUser("u1");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser }
            };

            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("u1");

            var result = controller.AddComment(1, "Nice outfit") as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Single(db.Comments);
            Assert.Equal("Nice outfit", db.Comments.First().Content);
        }

        [Fact]
        public void Like_TogglesVote()
        {
            var db = GetInMemoryDb();
            Outfit outfit = new Outfit();
            outfit.Image = "test.jpg";
            outfit.Name = "Test";
            outfit.User = new User() { Id = "u1", UserName = "TestUser" };
            outfit.Date_added = DateTime.Now;
            outfit.IsPublic = false;
            outfit.Description = "test";
            db.Outfits.Add(outfit);
            db.SaveChanges();

            var userId = "user123";
            var mockUserManager = GetMockUserManager();
            var controller = new OutfitController(db, mockUserManager.Object);

            var mockUser = GetMockUser(userId);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser }
            };

            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            // Like once
            controller.Like(1);
            Assert.Single(db.Votes);

            // Unlike
            controller.Like(1);
            Assert.Empty(db.Votes);
        }
    }
}
