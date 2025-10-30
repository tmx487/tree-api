using System.Text.Json;
using api.Domain;
using api.Domain.Abstractions;
using api.Domain.Exceptions;
using api.Domain.Repositories;

namespace api.Presentation.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger, IDateTimeProvider dateTimeProvider)
    {
        _next = next;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IExceptionJournalRepository journalRepository)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, journalRepository);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        IExceptionJournalRepository journalRepository)
    {
        var requestPath = context.Request.Path.Value ?? string.Empty;
        var httpMethod = context.Request.Method;
        var queryParams = CaptureQueryParameters(context);
        var bodyParams = await CaptureBodyParametersAsync(context);

        var isSecureException = exception is SecureException;
        var exceptionType = isSecureException
            ? exception.GetType().Name.Replace("Exception", "")
            : "Exception";

        var journalEntry = ExceptionJournalEntry.Create(
            _dateTimeProvider,
            exceptionType,
            exception.Message,
            exception.StackTrace ?? string.Empty,
            requestPath,
            httpMethod,
            queryParams,
            bodyParams);

        try
        {
            await journalRepository.AddAsync(journalEntry);
        }
        catch (Exception journalEx)
        {
            _logger.LogError(journalEx, "Failed to save exception to journal");
        }
        
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new
        {
            type = exceptionType,
            id = journalEntry.EventId.ToString(),
            data = new Dictionary<string, string>
            {
                ["message"] = isSecureException
                    ? exception.Message
                    : $"Internal server error ID = {journalEntry.EventId}"
            }
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);

        if (isSecureException)
        {
            _logger.LogWarning(exception,
                "SecureException occurred: {ExceptionType} - {Message} (EventId: {EventId})",
                exceptionType, exception.Message, journalEntry.EventId);
        }
        else
        {
            _logger.LogError(exception,
                "Unhandled exception occurred (EventId: {EventId})",
                journalEntry.EventId);
        }
    }

    private string CaptureQueryParameters(HttpContext context)
    {
        try
        {
            var queryParams = context.Request.Query
                .ToDictionary(q => q.Key, q => q.Value.ToString());
            return JsonSerializer.Serialize(queryParams);
        }
        catch
        {
            return "{}";
        }
    }

    private async Task<string> CaptureBodyParametersAsync(HttpContext context)
    {
        try
        {
            context.Request.EnableBuffering();

            context.Request.Body.Position = 0;
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            return string.IsNullOrWhiteSpace(body) ? "{}" : body;
        }
        catch
        {
            return "{}";
        }
    }
}