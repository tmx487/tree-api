using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;
using TreeAPI.Application.Abstractions;
using TreeAPI.Filters;

namespace TreeAPI.IntegrationTests;

public class TreeApiExceptionFilterTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IJournalService> _mockJournalService;
    private readonly Mock<ILogger<TreeApiExceptionFilter>> _mockLogger;
    private readonly HttpClient _client;

    public TreeApiExceptionFilterTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        _mockJournalService = new Mock<IJournalService>();
        _mockLogger = new Mock<ILogger<TreeApiExceptionFilter>>();

        _cofigureTexstingContext(_factory);
    }

    [Fact]
    public async Task TreeApiExceptionFilter_Should_ThrowExceptionOfType_Secure()
    {
        var testMessage = "Testing: secure exception";

        _mockJournalService
            .Setup(x => x.LogExceptionAsync(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Exception>(),
                It.IsAny<CancellationToken>()
                ))
            .Returns(Task.CompletedTask);

        var response = await _client.GetAsync($"/test/throw-secure-exception-query?message={testMessage}");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Equal("Secure", jsonResponse.GetProperty("type").GetString());
        Assert.True(jsonResponse.GetProperty("id").GetString()?.Length > 0);
        Assert.Equal(testMessage, jsonResponse.GetProperty("data").GetProperty("message").GetString());
    }

    private void _cofigureTexstingContext(WebApplicationFactory<Program> factory)
    {
        factory = new WebApplicationFactory<Program>()
               .WithWebHostBuilder(builder =>
               {
                   builder.ConfigureServices(services =>
                   {
                       var journalServiceDescriptor = services.SingleOrDefault(
                           d => d.ServiceType == typeof(IJournalService));
                       if (journalServiceDescriptor != null)
                           services.Remove(journalServiceDescriptor);

                       var loggerDescriptor = services.SingleOrDefault(
                           d => d.ServiceType == typeof(ILogger<TreeApiExceptionFilter>));
                       if (loggerDescriptor != null)
                           services.Remove(loggerDescriptor);

                       services.AddSingleton(_mockJournalService.Object);
                       services.AddSingleton(_mockLogger.Object);

                       services.AddControllers(options =>
                       {
                           options.Filters.Add<TreeApiExceptionFilter>();
                       });
                   });
               });
    }
}
