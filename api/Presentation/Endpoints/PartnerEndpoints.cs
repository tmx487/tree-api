using api.Application.Abstractions;
using api.Presentation.DTO;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Endpoints;

public static class PartnerEndpoints
{
    private const string TagName = "user.partner";
    
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api.user.partner")
            .WithTags(TagName)
            .WithOpenApi(opt =>
            {
                opt.Responses["200"].Description = "Successful response";
                return opt;
            });

        group.MapPost("rememberMe", RememberMe)
            .WithDescription(
                "(Optional) Saves user by unique code and returns auth token required on all other requests, if implemented.")
            .Produces<TokenInfo>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("add", Add)
            .WithDescription("Creates a  new Partner (User) and returns their ID.")
            .Accepts<AddPartnerRequest>("application/json")
            .Produces<PartnerResponse>(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> RememberMe(
        [FromQuery] string code,
        IPartnerService partnerService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Results.BadRequest("Authentication code is required.");
        }
        
        var tokenInfo = await partnerService.GetAuthTokenAsync(code, cancellationToken);
        if (string.IsNullOrWhiteSpace(tokenInfo.Token))
        {
            return Results.Unauthorized();
        }
        return Results.Ok(tokenInfo);
    }

    private static async Task<IResult> Add(
        [FromBody] AddPartnerRequest request, 
        IPartnerService partnerService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Results.BadRequest("Partner name is required.");
        }
        
        try
        {
            var result = await partnerService.AddAsync(
                request.Name,
                request.IsAdmin,
                cancellationToken);
    
            return Results.Ok(new PartnerResponse(result.PartnerId, result.AuthCode)); 
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }
}