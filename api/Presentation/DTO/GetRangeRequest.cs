using api.Application.DTO;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.DTO;

public record GetRangeRequest(
    [FromQuery] int Skip,
    [FromQuery] int Take,
    VJournalFilter? Filter
);