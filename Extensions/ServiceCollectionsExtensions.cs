using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TreeAPI.Application.Abstractions;
using TreeAPI.Application.Services;
using TreeAPI.Infrastructure.Persistence;

namespace TreeAPI.Extensions
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<ITreeService, TreeService>();
            services.AddScoped<IJournalService, JournalService>();
            return services;
        }

        public static IServiceCollection AddTreeDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var provider = "PostgreSQl";
            var connectionString = configuration.GetConnectionString(provider)
                ?? throw new NullReferenceException("ConnectionStrings section does not consist a needed string.");

            services.AddDbContext<TreeApiDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "TreeAPI",
                    Description = "API Description"
                });
                c.DocumentFilter<TagDescriptionsDocumentFilter>();
            });
            return services;
        }
    }

    internal class TagDescriptionsDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new List<OpenApiTag>
        {
            new OpenApiTag { Name = "user.journal", Description = "Represents journal API" },
            new OpenApiTag { Name = "user.partner", Description = "" },
            new OpenApiTag { Name = "user.tree", Description = "Represents entire tree API" },
            new OpenApiTag { Name = "user.tree.node", Description = "Represents tree node API" }
        };
        }
    }
}