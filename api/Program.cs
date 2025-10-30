using api.Infrastructure;
using api.Presentation.Endpoints;
using api.Presentation.Extenstions;
using api.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureJwtAuth(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.AddServices();

var app = builder.Build();

await app.ApplyMigrationsAsync<PsqlDbContext>();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapTreeEndpoints();
app.MapNodeEndpoints();
app.MapJournalEndpoints();
app.MapUserEndpoints();

app.Run();