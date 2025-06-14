using Dress_Up.Controllers;
using Dress_Up.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class ChatbotTest
{
    private Mock<IConfiguration> _mockConfig;

    private ApplicationDbContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        return new ApplicationDbContext(options);
    }

    private ChatbotController SetupControllerWithMockedHttpClient(Mock<HttpMessageHandler> handlerMock)
    {
        var httpClient = new HttpClient(handlerMock.Object);
        var controller = new ChatbotController(_mockConfig.Object)
        {
            ControllerContext = new ControllerContext()
        };
        //?????
        typeof(ChatbotController)
            .GetField("<client>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(controller, httpClient);

        return controller;
    }

    public ChatbotTest()
    {
        _mockConfig = new Mock<IConfiguration>();
    }

    [Fact]
    public async Task AskAssistant_ReturnsBadRequest_WhenApiFails()
    {
        // Arrange
        _mockConfig.Setup(c => c["OpenAI:ApiKey"]).Returns("fake-api-key");

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Eroare OpenAI")
            });

        var controller = SetupControllerWithMockedHttpClient(handlerMock);

        // Act
        var result = await controller.AskAssistant("Recomandă un outfit");

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Eroare", badRequest.Value.ToString());
    }
}
