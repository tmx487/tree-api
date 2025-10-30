using api.Application.Abstractions;
using api.Application.DTO;
using api.Presentation.DTO;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Endpoints;

public static class NodeEndpoints
{
    public static void MapNodeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api.user.tree.node")
            .WithTags("user.tree.node")
            .WithOpenApi(opt =>
            {
                opt.Responses["200"].Description = "Successful response";
                return opt;
            })
            .RequireAuthorization();

        group.MapPost("create", CreateNode)
            .WithDescription(
                "Create a new node in your tree." +
                "You must specify a parent node ID that belongs to your tree or" +
                "don't pass parent ID to create tree first level node." +
                "A new node name must be unique across all siblings.")
            .Produces(StatusCodes.Status200OK);

        group.MapPost("rename", RenameNode)
            .WithDescription(
                "Rename an existing node in your tree. " +
                "A new name of the node must be unique across all siblings.")
            .Produces(StatusCodes.Status200OK);

        group.MapPost("delete", DeleteNode)
            .WithDescription("Delete an existing node and all its descendants")
            .Produces(StatusCodes.Status200OK);
    }
    private static async Task<IResult> CreateNode(
        [FromQuery] string treeName,
        [FromQuery] long? parentNodeId,
        [FromQuery] string nodeName,
        [FromServices] INodeService nodeService,
        CancellationToken cancellationToken)
    {
        await nodeService.CreateNodeAsync(treeName, parentNodeId, nodeName, cancellationToken);
        return Results.Ok();
    }

    private static async Task<IResult> RenameNode(
        [FromQuery] long nodeId,
        [FromQuery] string newNodeName,
        [FromServices] INodeService nodeService,
        CancellationToken cancellationToken)
    {
        await nodeService.RenameNodeAsync(nodeId, newNodeName, cancellationToken);
        return Results.Ok();
    }

    private static async Task<IResult> DeleteNode(
        [FromQuery] long nodeId,
        [FromServices] INodeService nodeService,
        CancellationToken cancellationToken)
    {
        await nodeService.DeleteNodeAsync(nodeId, cancellationToken);
        return Results.Ok();
    }
}