using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using TreeAPI.Application.Abstractions;
using TreeAPI.Domain.Exceptions;

namespace TreeAPI.Filters
{
    public class TreeApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IJournalService _journalService;
        private readonly ILogger<TreeApiExceptionFilter> _logger;

        public TreeApiExceptionFilter(IJournalService journalService, ILogger<TreeApiExceptionFilter> logger)
        {
            _journalService = journalService;
            _logger = logger;
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            var eventId = DateTime.UtcNow.Ticks;
            var request = context.HttpContext.Request;

            string? queryParams = string.Empty;
            if (request.Query.Any())
            {
                var paramsF = request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
                queryParams = JsonSerializer.Serialize(paramsF);
            }

            string? bodyParams = string.Empty;
            if (request.Body.CanRead && request.ContentLength > 0)
            {
                request.EnableBuffering();
                using (var reader = new System.IO.StreamReader(request.Body, System.Text.Encoding.UTF8, true, 1024, true))
                {
                    bodyParams = await reader.ReadToEndAsync();
                    request.Body.Position = 0;
                }
            }

            await _journalService.LogExceptionAsync(eventId, queryParams, bodyParams, context.Exception, context.HttpContext.RequestAborted);

            if (context.Exception is SecureException secureException)
            {
                context.Result = new ObjectResult(new
                {
                    type = secureException.GetType().Name.Replace("Exception", ""),
                    id = eventId.ToString(),
                    data = new { message = secureException.Message }
                })
                {
                    StatusCode = 500
                };
            }
            else
            {
                _logger.LogError(context.Exception, "Unhandled exception occurred. Event ID: {EventId}", eventId);
                context.Result = new ObjectResult(new
                {
                    type = "Exception",
                    id = eventId.ToString(),
                    data = new { message = $"Internal server error ID = {eventId}" }
                })
                {
                    StatusCode = 500
                };
            }

            context.ExceptionHandled = true;
        }
    }
}
