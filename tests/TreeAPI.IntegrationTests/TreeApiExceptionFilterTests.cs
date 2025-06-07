using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;
using TreeAPI.Application.Abstractions;
using TreeAPI.Filters;
using TreeAPI.Infrastructure.Persistence;

namespace TreeAPI.IntegrationTests;

public class TreeApiExceptionFilterTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly Mock<IJournalService> _mockJournalService;
    private readonly Mock<ILogger<TreeApiExceptionFilter>> _mockLogger;
    private readonly TreeApiDbContext _context;
    private IDbContextTransaction _transaction;
    private readonly HttpClient _client;

    public TreeApiExceptionFilterTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        _mockJournalService = _factory.MockJournalService;
        _mockLogger = _factory.MockLogger;

        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<TreeApiDbContext>();

        _transaction = _context.Database.BeginTransaction();

        _mockJournalService
            .Setup(x => x.LogExceptionAsync(
                It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Exception>(), It.IsAny<CancellationToken>()
            ))
            .Returns(Task.CompletedTask)
            .Verifiable();
    }

    public void Dispose()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();

        _context?.Dispose();
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
