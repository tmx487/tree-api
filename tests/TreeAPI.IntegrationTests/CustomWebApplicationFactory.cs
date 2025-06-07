using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data.Common;
using TreeAPI.Application.Abstractions;
using TreeAPI.Filters;
using TreeAPI.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace TreeAPI.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IJournalService> MockJournalService { get; } = new Mock<IJournalService>();
        public Mock<ILogger<TreeApiExceptionFilter>> MockLogger { get; } = new Mock<ILogger<TreeApiExceptionFilter>>();
        public DbConnection Connection { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TreeApiDbContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                var actualDbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(TreeApiDbContext));
                if (actualDbContextDescriptor != null)
                    services.Remove(actualDbContextDescriptor);

                Connection = new Npgsql
                .NpgsqlConnection("Host=localhost;Port=9432;Database=tree_db;Username=tree_dba;Password=tree_dba_21278;");
                Connection.Open();

                services.AddDbContext<TreeApiDbContext>(options =>
                {
                    options.UseNpgsql(Connection);
                });

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<TreeApiDbContext>();
                    context.Database.EnsureCreated();
                }

                var journalServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IJournalService));
                if (journalServiceDescriptor != null)
                    services.Remove(journalServiceDescriptor);
                services.AddSingleton(MockJournalService.Object);

                var loggerDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ILogger<TreeApiExceptionFilter>));
                if (loggerDescriptor != null)
                    services.Remove(loggerDescriptor);
                services.AddSingleton(MockLogger.Object);
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Connection?.Close();
                Connection?.Dispose();
            }
        }
    }
}