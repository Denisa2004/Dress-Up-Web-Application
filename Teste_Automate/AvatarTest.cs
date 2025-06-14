using Dress_Up.Controllers;
using Dress_Up.Data;
using Dress_Up.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

public class AvatarTest
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<AchievementService> _mockAchievementService;

    public AvatarTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        var store = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

        _mockAchievementService = new Mock<AchievementService>(_context);
    }

    [Fact]
    public void Index_WhenNoAvatars_AddsDefaultAvatars()
    {
        // Asigură-te că baza este goală
        Assert.Empty(_context.Avatars);

        var controller = new AvatarController(_context, _mockUserManager.Object, _mockAchievementService.Object);

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult);

        // Verifică dacă s-au adăugat avatarurile default
        Assert.NotEmpty(_context.Avatars);
        Assert.True(_context.SaveChanges() >= 0);  // SaveChanges poate returna 0 sau mai mult
    }

    [Fact]
    public void Personalizare_ReturnsViewWithAvatarId()
    {
        var controller = new AvatarController(_context, _mockUserManager.Object, _mockAchievementService.Object);

        var result = controller.Personalizare(3);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(3, viewResult.ViewData["AvatarId"]);
    }


}
