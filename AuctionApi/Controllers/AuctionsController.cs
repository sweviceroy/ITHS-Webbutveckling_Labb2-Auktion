using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auction.Core.DTOs;
using Auction.Core.Interfaces;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionService _auctions;

    public AuctionsController(IAuctionService auctions) => _auctions = auctions;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<AuctionDetailDto>> Create(CreateAuctionDto dto)
    {
        try
        {
            var userId = GetUserId();
            var result = await _auctions.CreateAuctionAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionListDto>>> GetAll([FromQuery] string? title, [FromQuery] bool includeClosed = false)
    {
        return Ok(await _auctions.GetAuctionsAsync(title, includeClosed));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDetailDto>> GetById(Guid id)
    {
        var auction = await _auctions.GetAuctionByIdAsync(id);
        return auction == null ? NotFound() : Ok(auction);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<AuctionDetailDto>> Update(Guid id, UpdateAuctionDto dto)
    {
        try { return Ok(await _auctions.UpdateAuctionAsync(id, dto, GetUserId())); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
