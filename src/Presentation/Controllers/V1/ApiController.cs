using Application.Blockchain.Commands;
using Application.Blockchain.Queries;
using Application.Common.Abstractions;
using Application.Dto;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.V1;

[Authorize]
[ApiController]
[Route("/api/v1/")]
[Produces("application/json")]
public class ApiController(ISender mediator, ILogger<ApiController> logger, IProcessedVotesCache processedVotesCache) : ControllerBase
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

    [HttpPost("cast_vote")]
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

    [HttpGet("stats/counts")]
    public async Task<ActionResult<Dictionary<Party, int>>> GetPartyVoteCounts(CancellationToken ct)
    {
        var query = new GetPartyVotes();
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpGet("stats/daily")]
    public async Task<ActionResult<Dictionary<Party, Dictionary<DateOnly, int>>>> GetPartyDailyVoteCounts(CancellationToken ct)
    {
        var query = new GetDailyPartyVotes();
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpGet("voters/{address}")]
    public async Task<ActionResult<VoterDto>> GetVoterByAddress(string address, CancellationToken ct)
    {
        var query = new GetVoterByAddressQuery(address);
        var voterInfo = await mediator.Send(query, ct);
        return voterInfo is not null
            ? Ok(voterInfo)
            : NotFound();
    }

    // votes/hash
    [HttpGet("votes/{hash}")]
    public async Task<ActionResult<VoteDto>> GetVoteByHash(string hash, CancellationToken ct)
    {
        var query = new GetVoteByHashQuery(hash);
        var vote = await mediator.Send(query, ct);

        return vote is not null
            ? Ok(vote)
            : NotFound();
    }

    [HttpGet("blocks/{index:long}")]
    public async Task<ActionResult<BlockDto>> GetBlockByIndex(long index, CancellationToken ct)
    {
        var query = new GetBlockByIndexQuery(index);
        var block = await mediator.Send(query, ct);

        return block is not null
            ? Ok(block)
            : NotFound();
    }

    [HttpGet("blocks/{hash}")]
    public async Task<ActionResult<BlockDto>> GetBlockByHash(string hash, CancellationToken ct)
    {
        var query = new GetBlockByHashQuery(hash);
        var block = await mediator.Send(query, ct);

        return block is not null
            ? Ok(block)
            : NotFound();
    }
}