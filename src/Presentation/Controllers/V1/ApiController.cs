using Application.Blockchain.Commands;
using Application.Blockchain.Queries;
using Application.Common.Abstractions;
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
public class ApiController(
    IMediator mediator,
    ILogger<ApiController> logger,
    IProcessedVotesCache processedVotesCache,
    IDateTimeProvider dateTimeProvider) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("d831889244f1484d8f4797a3a85a4807")]
    public IActionResult GetNewIdentity()
    {
        var voter = Voter.NewVoter();
        var resp = new
        {
            voter.Address,
            voter.PublicKey,
            voter.PrivateKey,
        };

        Response.Cookies.Append(PublicKeyBearerAuthHandler.PubKeyHeaderName, voter.PublicKey);
        Response.Cookies.Append(PublicKeyBearerAuthHandler.SignatureHeaderName, voter.GenerateAddressSignature());

        return Ok(resp);
    }

    [HttpGet("user_claims")]
    public ActionResult<Dictionary<string, string>> GetUserClaims()
    {
        return Ok(User.Claims.ToDictionary(x => x.Type, x => x.Value));
    }

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

    [HttpGet("blocks/all")]
    public async Task<ActionResult<IEnumerable<Block>>> GetAllBlocks([FromQuery] int page, CancellationToken ct)
    {
        var query = new GetAllBlocksQuery(page);
        var blocks = await mediator.Send(query, ct);
        return Ok(blocks);
    }

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

    // TODO remove
    [HttpGet("new_identity")]
    public IActionResult NewIdentity()
    {
        if (HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>() is var env && !env.IsDevelopment())
            return NotFound();

        var voter = Voter.NewVoter();

        var votes = Enumerable.Range(5, 2)
            .Select(i =>
            {
                var vote = Vote.NewVote(voter, i, dateTimeProvider.UtcNow);
                vote.Mine();
                return new
                {
                    hash = vote.Hash,
                    pkey = voter.PublicKey,
                    sig = vote.Signature,
                    party_id = vote.PartyId,
                    timestamp = vote.UnixTimestampMs,
                    nonce = vote.Nonce,
                };
            });

        return Ok(new
        {
            addr = voter.Address,
            pkey = voter.PublicKey,
            addr_signed = voter.Sign(voter.Address.ToBytesFromHex()).ToHexString(),
            votes,
        });
    }

    // TODO remove
    [HttpPost("vote_random")]
    public async Task<IActionResult> VoteRandom(CancellationToken ct)
    {
        if (HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>() is var env && !env.IsDevelopment())
            return NotFound();

        var voter = Voter.NewVoter();
        var vote = Vote.NewVote(voter, Random.Shared.Next(1, 100), dateTimeProvider.UtcNow);
        vote.Mine();

        // override the test voter
        HttpContext.Items[nameof(Voter)] = voter;

        var command = new CastVoteCommand(vote.Hash, voter.PublicKey, vote.Signature, vote.PartyId, vote.UnixTimestampMs, vote.Nonce);

        await mediator.Send(command, ct);

        return Created();
    }
}