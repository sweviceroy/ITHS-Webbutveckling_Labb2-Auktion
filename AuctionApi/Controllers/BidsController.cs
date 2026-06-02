using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuctionApi.DTOs;
using AuctionApi.Models;
using AuctionApi.Repositories;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/auctions/{auctionId}/[controller]")]
public class BidsController : ControllerBase
{
    private readonly IAuctionRepository _auctions;
    private readonly IBidRepository _bids;
    private readonly IUserRepository _users;

    public BidsController(IAuctionRepository auctions, IBidRepository bids, IUserRepository users)
    {
        _auctions = auctions;
        _bids = bids;
        _users = users;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BidDto>> PlaceBid(Guid auctionId, CreateBidDto dto)
    {
        var auction = await _auctions.GetByIdAsync(auctionId);

        if (auction == null)
            return NotFound();

        if (!auction.IsActive || auction.EndDate <= DateTime.UtcNow)
            return BadRequest(new { message = "Auction is closed" });

        var userId = GetUserId();

        if (auction.CreatorId == userId)
            return BadRequest(new { message = "You cannot bid on your own auction" });

        var bids = await _bids.GetBidsByAuctionAsync(auctionId);
        var highestBid = bids.MaxBy(b => b.Amount);
        var minimumBid = highestBid != null ? highestBid.Amount : auction.StartingPrice;

        if (dto.Amount <= minimumBid)
            return BadRequest(new { message = $"Bid must be higher than {minimumBid}" });

        var bid = new Bid
        {
            Amount = dto.Amount,
            AuctionId = auctionId,
            BidderId = userId
        };

        await _bids.CreateAsync(bid);
        auction.CurrentHighestBid = dto.Amount;
        await _auctions.UpdateAsync(auction);

        var bidder = await _users.GetByIdAsync(userId);

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
        var bids = await _bids.GetBidsByAuctionAsync(auctionId);

        return Ok(bids.Select(b => new BidDto
        {
            Id = b.Id,
            Amount = b.Amount,
            BidTime = b.BidTime,
            BidderUsername = b.Bidder.Username,
            BidderId = b.BidderId
        }).ToList());
    }

    [HttpDelete("latest")]
    [Authorize]
    public async Task<IActionResult> UndoLatestBid(Guid auctionId)
    {
        var auction = await _auctions.GetByIdAsync(auctionId);

        if (auction == null)
            return NotFound();

        if (!auction.IsActive || auction.EndDate <= DateTime.UtcNow)
            return BadRequest(new { message = "Auction is closed" });

        var userId = GetUserId();
        var latestBid = await _bids.GetLatestBidAsync(auctionId);

        if (latestBid == null)
            return BadRequest(new { message = "No bids to undo" });

        if (latestBid.BidderId != userId)
            return BadRequest(new { message = "You can only undo your own latest bid" });

        await _bids.DeleteAsync(latestBid);

        var remainingBids = await _bids.GetBidsByAuctionAsync(auctionId);
        auction.CurrentHighestBid = remainingBids
            .Where(b => b.Id != latestBid.Id)
            .MaxBy(b => (decimal?)b.Amount)?.Amount;
        await _auctions.UpdateAsync(auction);

        return Ok(new { message = "Bid undone" });
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
