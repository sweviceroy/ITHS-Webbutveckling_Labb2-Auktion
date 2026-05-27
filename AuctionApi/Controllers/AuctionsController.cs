using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuctionApi.Data;
using AuctionApi.DTOs;
using AuctionApi.Models;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _db;

    public AuctionsController(AuctionDbContext db)
    {
        _db = db;
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

        _db.Auctions.Add(auction);
        await _db.SaveChangesAsync();

        await _db.Entry(auction).Reference(a => a.Creator).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = auction.Id }, MapDetailDto(auction));
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionListDto>>> GetAll([FromQuery] string? title, [FromQuery] bool includeClosed = false)
    {
        var query = _db.Auctions
            .Include(a => a.Creator)
            .Include(a => a.Bids)
            .Where(a => a.IsActive);

        if (!includeClosed)
            query = query.Where(a => a.EndDate > DateTime.UtcNow);

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(a => a.Title.Contains(title));

        var auctions = await query
            .OrderByDescending(a => a.EndDate)
            .Select(a => new AuctionListDto
            {
                Id = a.Id,
                Title = a.Title,
                ImageUrl = a.ImageUrl,
                StartingPrice = a.StartingPrice,
                CurrentHighestBid = a.CurrentHighestBid,
                EndDate = a.EndDate,
                IsOpen = a.EndDate > DateTime.UtcNow,
                BidCount = a.Bids.Count,
                CreatorUsername = a.Creator.Username
            })
            .ToListAsync();

        return Ok(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDetailDto>> GetById(Guid id)
    {
        var auction = await _db.Auctions
            .Include(a => a.Creator)
            .Include(a => a.Bids)
                .ThenInclude(b => b.Bidder)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (auction == null)
            return NotFound();

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
        CreatorUsername = a.Creator.Username,
        Bids = a.Bids.OrderByDescending(b => b.BidTime).Select(b => new BidDto
        {
            Id = b.Id,
            Amount = b.Amount,
            BidTime = b.BidTime,
            BidderUsername = b.Bidder.Username,
            BidderId = b.BidderId
        }).ToList()
    };
}
