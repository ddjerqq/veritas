using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common.Abstractions;

namespace Presentation.Controllers.V1;

/// <summary>
/// controller for authentication
/// </summary>
[Authorize]
[Route("/api/v1/[controller]")]
public class AuthController : ApiController
{
    [Authorize]
    [HttpGet("user_claims")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<Dictionary<string, string>> GetUserClaims()
    {
        return Ok(User.Claims.ToDictionary(x => x.Type, x => x.Value));
    }
}