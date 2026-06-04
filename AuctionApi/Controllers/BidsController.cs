using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auction.Core.DTOs;
using Auction.Core.Interfaces;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/auctions/{auctionId}/[controller]")]
public class BidsController : ControllerBase
{
    private readonly IBidService _bids;

    public BidsController(IBidService bids) => _bids = bids;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BidDto>> PlaceBid(Guid auctionId, CreateBidDto dto)
    {
        try { return Ok(await _bids.PlaceBidAsync(auctionId, dto.Amount, GetUserId())); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet]
    public async Task<ActionResult<List<BidDto>>> GetBids(Guid auctionId)
    {
        return Ok(await _bids.GetBidsAsync(auctionId));
    }

    [HttpDelete("latest")]
    [Authorize]
    public async Task<IActionResult> UndoLatestBid(Guid auctionId)
    {
        try
        {
            await _bids.UndoLatestBidAsync(auctionId, GetUserId());
            return Ok(new { message = "Bid undone" });
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
