
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dress_Up.Controllers;
using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

public class EventTest
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        return context;
    }

    [Fact]
    public async Task CanCreateNewEvent()
    {
        // Arrange
        var db = GetInMemoryDbContext();
        var achievementService = new Mock<AchievementService>(null);
        var controller = new EventController(db, achievementService.Object);
        var newEvent = new Event { Id = 1, Name = "TestEvent", Description = "TestDesc", Image = "test.jpg" };

        // Act
        var result = await controller.New(newEvent) as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Single(db.Events);
    }

    [Fact]
    public async Task CanShowEventDetails()
    {
        var db = GetInMemoryDbContext();
        db.Events.Add(new Event { Id = 1, Name = "ShowEvent", Description="Test", Image="test.jpg" });
        db.SaveChanges();

        var controller = new EventController(db, new Mock<AchievementService>(null).Object);
        var result = await controller.Show(1) as ViewResult;

        Assert.NotNull(result);
        Assert.IsType<ViewResult>(result);
    }


    [Fact]
    public async Task CanSubmitVote()
    {
        var db = GetInMemoryDbContext();
        var user = new User { Id = "voter1", UserName = "voteruser",
            Email = "testuser@email.com",
            NormalizedUserName = "TESTUSER"
        };
        var ev = new Event { Id = 20, Name = "VotingEvent", IsActive = true, Description = "Test", Image = "test.jpg" };
        var outfit = new Outfit { Id = 2, Name = "VoteOutfit", User = user };
        db.Users.Add(user);
        db.Events.Add(ev);
        db.Outfits.Add(outfit);
        db.SaveChanges();

        var controller = new EventController(db, new Mock<AchievementService>(null).Object);
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "voteruser")
            }, "mock"));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var result = await controller.SubmitVote(20, 2) as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Single(db.Votes);
        Assert.Equal("Show", result.ActionName);
        Assert.Equal("Event", result.ControllerName);
    }
}
