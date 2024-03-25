using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common.Abstractions;

namespace Presentation.Controllers.V1;

/// <summary>
/// controller for authentication
/// </summary>
[Authorize]
[Route("/api/v1/[controller]")]
public sealed class AuthController : ApiController
{
    // [Authorize]
    // [HttpGet("user_claims")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public ActionResult<Dictionary<string, string>> GetUserClaims()
    // {
    //     return Ok(User.Claims.ToDictionary(x => x.Type, x => x.Value));
    // }
    //
    // [AllowAnonymous]
    // [HttpPost("register")]
    // [ProducesResponseType(StatusCodes.Status201Created)]
    // public async Task<IActionResult> Register([FromBody] RegisterCommand request, CancellationToken ct = default)
    // {
    //     var result = await Mediator.Send(request, ct);
    //
    //     if (!result.Succeeded)
    //         return BadRequest(result.ToProblemDetails());
    //
    //     return Created();
    // }
}