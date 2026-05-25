using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuctionApi.Data;
using AuctionApi.DTOs;
using AuctionApi.Models;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/auctions/{auctionId}/[controller]")]
public class BidsController : ControllerBase
{
    private readonly AuctionDbContext _db;

    public BidsController(AuctionDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BidDto>> PlaceBid(Guid auctionId, CreateBidDto dto)
    {
        var auction = await _db.Auctions
            .Include(a => a.Bids)
            .FirstOrDefaultAsync(a => a.Id == auctionId);

        if (auction == null)
            return NotFound();

        if (!auction.IsActive || auction.EndDate <= DateTime.UtcNow)
            return BadRequest(new { message = "Auction is closed" });

        var userId = GetUserId();

        if (auction.CreatorId == userId)
            return BadRequest(new { message = "You cannot bid on your own auction" });

        var highestBid = auction.Bids.MaxBy(b => b.Amount);
        var minimumBid = highestBid != null ? highestBid.Amount : auction.StartingPrice;

        if (dto.Amount <= minimumBid)
            return BadRequest(new { message = $"Bid must be higher than {minimumBid}" });

        var bid = new Bid
        {
            Amount = dto.Amount,
            AuctionId = auctionId,
            BidderId = userId
        };

        _db.Bids.Add(bid);
        auction.CurrentHighestBid = dto.Amount;
        await _db.SaveChangesAsync();

        var bidder = await _db.Users.FindAsync(userId);

        return Ok(new BidDto
        {
            Id = bid.Id,
            Amount = bid.Amount,
            BidTime = bid.BidTime,
            BidderUsername = bidder!.Username,
            BidderId = bidder.Id
        });
    }

    [HttpGet]
    public async Task<ActionResult<List<BidDto>>> GetBids(Guid auctionId)
    {
        var auctionExists = await _db.Auctions.AnyAsync(a => a.Id == auctionId);
        if (!auctionExists)
            return NotFound();

        var bids = await _db.Bids
            .Include(b => b.Bidder)
            .Where(b => b.AuctionId == auctionId)
            .OrderByDescending(b => b.BidTime)
            .Select(b => new BidDto
            {
                Id = b.Id,
                Amount = b.Amount,
                BidTime = b.BidTime,
                BidderUsername = b.Bidder.Username,
                BidderId = b.BidderId
            })
            .ToListAsync();

        return Ok(bids);
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
