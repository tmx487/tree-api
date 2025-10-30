using api.Application.Abstractions;
using api.Application.DTO;
using api.Presentation.DTO;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Endpoints;

public static class TreeEndpoints
{
    private const string TagName = "user.tree";
    public static void MapTreeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("api.user.tree.get", GetTree)
            .WithTags(TagName)
            .WithDescription("Returns your entire tree. If your tree doesn't exist it will be created automatically.")
            .WithOpenApi(opt =>
            {
                opt.Responses["200"].Description = "Successful response";
                return opt;
            })
            .Produces<NodeDto>(StatusCodes.Status200OK, "application/json")
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();
    }

    private static async Task<IResult> GetTree(
        [FromQuery] string treeName,
        [FromServices] INodeService nodeService,
        CancellationToken cancellationToken)
    {
        var result = await nodeService.GetTreeAsync(treeName, cancellationToken);
        return Results.Ok(result);
    }
}