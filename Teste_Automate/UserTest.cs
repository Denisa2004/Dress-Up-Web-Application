namespace Teste_Automate
{
    using Xunit;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Linq;
    using Dress_Up.Models;
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using Dress_Up.Data;
    using Microsoft.EntityFrameworkCore;
    using Dress_Up.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;

    public class UserTest
    {
        [Fact]
        public async Task Index_UserExists_ReturnsViewWithUser()
        {
            // Arrange
            var mockUserManager = MockUserManager<User>(new List<User>());
            var mockSignInManager = MockSignInManager<User>();
            var mockAchievementService = new Mock<AchievementService>(null); 
            var mockContext = new Mock<ApplicationDbContext>();

            var userId = "user-123";
            var user = new User { Id = userId, UserName = "testuser" };

            // Mock Users DbSet
            var users = new List<User> { user }.AsQueryable();
            var mockUsersDbSet = new Mock<DbSet<User>>();
            mockUsersDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUsersDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUsersDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUsersDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            mockContext.Setup(c => c.Users).Returns(mockUsersDbSet.Object);

            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var controller = new UserController(mockContext.Object, mockUserManager.Object, mockSignInManager.Object, mockAchievementService.Object);

            // Trebuie să setăm User în Controller ca să funcționeze GetUserId(User)
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            // Mock serviciul de achievement (returnează o listă goală ca exemplu)
            //mockAchievementService.Setup(a => a.GetUserAchievementsAsync(userId)).ReturnsAsync(new List<string>());

            // Act
            var result = await controller.Index(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<User>(viewResult.Model);
            Assert.Equal(userId, model.Id);
        }

        // Helper pentru mock UserManager
        private Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> users) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns((ClaimsPrincipal cp) =>
            {
                var idClaim = cp.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                return idClaim?.Value;
            });
            return mgr;
        }

        // Helper pentru mock SignInManager
        private Mock<SignInManager<TUser>> MockSignInManager<TUser>() where TUser : class
        {
            var userManager = MockUserManager<TUser>(new List<TUser>());
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
            return new Mock<SignInManager<TUser>>(userManager.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);
        }
    }

}
