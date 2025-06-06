
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using TreeAPI.Extensions;
using TreeAPI.Filters;

namespace TreeAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<TreeApiExceptionFilter>();
            });

            builder.Services.AddScoped<TreeApiExceptionFilter>();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwagger();

            builder.Services.AddTreeDbContext(builder.Configuration);

            builder.Services.AddCustomServices();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swagger, httpReq) =>
                    {
                        var updatedPaths = new OpenApiPaths();
                        foreach (var path in swagger.Paths)
                        {
                            updatedPaths.Add(path.Key.ToLowerInvariant(), path.Value);
                        }
                        swagger.Paths = updatedPaths;
                    });

                });
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
