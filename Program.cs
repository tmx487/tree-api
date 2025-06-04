
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using TreeAPI.Extensions;

namespace TreeAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            
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
