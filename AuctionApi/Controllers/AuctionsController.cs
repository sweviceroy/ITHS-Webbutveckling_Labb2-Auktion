using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuctionApi.DTOs;
using AuctionApi.Models;
using AuctionApi.Repositories;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionRepository _auctions;

    public AuctionsController(IAuctionRepository auctions)
    {
        _auctions = auctions;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<AuctionDetailDto>> Create(CreateAuctionDto dto)
    {
        if (dto.EndDate <= dto.StartDate)
            return BadRequest(new { message = "End date must be after start date" });

        if (dto.EndDate <= DateTime.UtcNow)
            return BadRequest(new { message = "End date must be in the future" });

        var userId = GetUserId();

        var auction = new Auction
        {
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            StartingPrice = dto.StartingPrice,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            CreatorId = userId
        };

        await _auctions.CreateAsync(auction);
        var created = (await _auctions.GetByIdAsync(auction.Id))!;

        return CreatedAtAction(nameof(GetById), new { id = auction.Id }, MapDetailDto(created));
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionListDto>>> GetAll([FromQuery] string? title, [FromQuery] bool includeClosed = false)
    {
        var auctions = await _auctions.GetAllAsync(title, includeClosed);
        return Ok(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDetailDto>> GetById(Guid id)
    {
        var auction = await _auctions.GetByIdWithBidsAsync(id);

        if (auction == null)
            return NotFound();

        return Ok(MapDetailDto(auction));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<AuctionDetailDto>> Update(Guid id, UpdateAuctionDto dto)
    {
        var auction = await _auctions.GetByIdWithBidsAsync(id);

        if (auction == null)
            return NotFound();

        var userId = GetUserId();
        if (auction.CreatorId != userId)
            return Forbid();

        if (!auction.IsActive || auction.EndDate <= DateTime.UtcNow)
            return BadRequest(new { message = "Cannot update a closed auction" });

        auction.Title = dto.Title;
        auction.Description = dto.Description;
        auction.ImageUrl = dto.ImageUrl;
        auction.EndDate = dto.EndDate;

        if (dto.StartingPrice.HasValue)
        {
            if (auction.Bids.Count > 0)
                return BadRequest(new { message = "Cannot change starting price once bids have been placed" });
            auction.StartingPrice = dto.StartingPrice.Value;
        }

        await _auctions.UpdateAsync(auction);

        return Ok(MapDetailDto(auction));
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    private static AuctionDetailDto MapDetailDto(Auction a) => new()
    {
        Id = a.Id,
        Title = a.Title,
        Description = a.Description,
        ImageUrl = a.ImageUrl,
        StartingPrice = a.StartingPrice,
        CurrentHighestBid = a.CurrentHighestBid,
        StartDate = a.StartDate,
        EndDate = a.EndDate,
        IsOpen = a.EndDate > DateTime.UtcNow && a.IsActive,
        CreatorId = a.CreatorId,
        CreatorUsername = a.Creator?.Username ?? string.Empty,
        Bids = a.Bids?.OrderByDescending(b => b.BidTime).Select(b => new BidDto
        {
            Id = b.Id,
            Amount = b.Amount,
            BidTime = b.BidTime,
            BidderUsername = b.Bidder?.Username ?? string.Empty,
            BidderId = b.BidderId
        }).ToList() ?? new()
    };
}
