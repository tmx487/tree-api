using api.Application;
using api.Application.DTO;
using api.Presentation.DTO;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Endpoints;

public static class JournalEndpoints
{
    private const string TagName = "user.journal";

    public static void MapJournalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api.user.journal")
            .WithTags(TagName)
            .WithOpenApi(opt =>
            {
                opt.Responses["200"].Description = "Successful response";
                return opt;
            })
            .RequireAuthorization();

        group.MapPost("/getRange", GetRange)
            .WithDescription("Provides the pagination API. Skip means the number of items should be\n" +
                             "skipped by server. Take means the maximum number items should be\n+" +
                             "returned by server. All fields of the filter are optional.")
            .Produces<MRangeJournalInfo>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/getSingle", GetSingle)
            .WithDescription("Returns the information about an particular event by ID.")
            .Produces<MJournal>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetRange(
        [AsParameters] GetRangeRequest parameters,
        IExceptionJournalService journalService,
        CancellationToken cancellationToken)
    {
        if (parameters.Skip < 0 || parameters.Take <= 0)
        {
            return Results.BadRequest("Skip and Take must be positive.");
        }
        
        var effectiveFilter = parameters.Filter ?? new VJournalFilter(
            From: null,
            To: null,
            Search: string.Empty);
        
        var result = await journalService.GetRangeAsync(
            parameters.Skip,
            parameters.Take,
            effectiveFilter,
            cancellationToken);

        return Results.Ok(result);
    }

    private static async Task<IResult> GetSingle(
        long id,
        IExceptionJournalService journalService,
        CancellationToken cancellationToken)
    {
        // TODO: проверить: может ли ответ быть типа NULL
        var result = await journalService.GetSingleAsync(id, cancellationToken);

        if (result is null)
        {
            return Results.NotFound($"Journal entry with Event ID '{id}' not found.");
        }

        return Results.Ok(result);
    }
}