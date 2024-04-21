using Application.Blockchain.Commands;
using Application.Blockchain.Queries;
using Application.Common.Abstractions;
using Application.Dto;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Auth;

namespace Presentation.Controllers.V1;

[Authorize]
[ApiController]
[Route("/api/v1/")]
[Produces("application/json")]
public class ApiController(IMediator mediator, ILogger<ApiController> logger, IProcessedVotesCache processedVotesCache) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("new_identity")]
    public ActionResult<FullVoterDto> GetNewIdentity()
    {
        if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            return Unauthorized();

        var voter = Voter.NewVoter();
        var dto = new FullVoterDto(voter.Address, voter.PublicKey, voter.PrivateKey!, voter.SignAddress());

        Response.Cookies.Append(nameof(FullVoterDto.Address), dto.Address);
        Response.Cookies.Append(nameof(FullVoterDto.PublicKey), dto.PublicKey);
        Response.Cookies.Append(nameof(FullVoterDto.PrivateKey), dto.PrivateKey);
        Response.Cookies.Append(nameof(FullVoterDto.Signature), dto.Signature);

        return Ok(dto);
    }

    // [HttpGet("user_claims")]
    // public ActionResult<Dictionary<string, string>> GetUserClaims()
    // {
    //     return Ok(User.Claims.ToDictionary(x => x.Type, x => x.Value));
    // }

    [HttpPost("votes")]
    public async Task<IActionResult> CastVote(CastVoteCommand command, CancellationToken ct)
    {
        if (processedVotesCache.Contains(command.Hash))
        {
            logger.LogWarning("Tried processing the same vote twice: {Hash}", command.Hash);
            return BadRequest("The vote has already been processed");
        }

        processedVotesCache.Add(command.Hash);
        await mediator.Send(command, ct);
        return Created();
    }

    [HttpGet("voters/{address}")]
    public async Task<ActionResult<VoterInfo>> GetVoterInfo(string address, CancellationToken ct)
    {
        var query = new GetVoterInfoQuery(address);
        var voterInfo = await mediator.Send(query, ct);
        return voterInfo is not null
            ? Ok(voterInfo)
            : NotFound();
    }

    // [HttpGet("blocks/all")]
    // public async Task<ActionResult<IEnumerable<Block>>> GetAllBlocks([FromQuery] int page, CancellationToken ct)
    // {
    //     var query = new GetAllBlocksQuery(page);
    //     var blocks = await mediator.Send(query, ct);
    //     return Ok(blocks);
    // }

    [HttpGet("blocks/{index:long}")]
    public async Task<ActionResult<IEnumerable<Block>>> GetBlockByIndex(long index, CancellationToken ct)
    {
        var query = new GetBlockByIndexQuery(index);
        var block = await mediator.Send(query, ct);

        return block is not null
            ? Ok(block)
            : NotFound();
    }

    [HttpGet("blocks/{hash}")]
    public async Task<ActionResult<IEnumerable<Block>>> GetBlockByHash(string hash, CancellationToken ct)
    {
        var query = new GetBlockByHashQuery(hash);
        var block = await mediator.Send(query, ct);

        return block is not null
            ? Ok(block)
            : NotFound();
    }
}