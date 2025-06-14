using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using Dress_Up.Controllers;
using Dress_Up.Data;
using Dress_Up.Models;
using Moq;

namespace Teste_Automate
{
    public class AdminTest
    {
        private static Mock<UserManager<User>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private ApplicationDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void SendMessage_Get_ReturnsViewWithUsers()
        {
            // Arrange
            var context = GetInMemoryDbContext("SendMessage_Get_DB");
            context.Users.Add(new User { Id = "1", UserName = "AdminUser" });
            context.SaveChanges();

            var controller = new AdminController(context, GetMockUserManager().Object);

            // Act
            var result = controller.SendMessage();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var users = Assert.IsAssignableFrom<List<User>>(viewResult.ViewData["Users"]);
            Assert.Single(users);
            Assert.Equal("AdminUser", users.First().UserName);
        }


        [Fact]
        public void SendMessage_Post_InvalidMessage_DoesNotSave()
        {
            // Arrange
            var context = GetInMemoryDbContext("SendMessage_Post_Invalid");
            var controller = new AdminController(context, GetMockUserManager().Object);

            // Act
            var result = controller.SendMessage("", "");
            var messages = context.AlertMessages.ToList();

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SendMessage", redirect.ActionName);
            Assert.Empty(messages);
        }
    }
}
