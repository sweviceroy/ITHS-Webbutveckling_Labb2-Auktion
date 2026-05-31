using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuctionApi.Data;
using AuctionApi.DTOs;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AuctionDbContext _db;

    public AdminController(AuctionDbContext db)
    {
        _db = db;
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _db.Users
            .OrderBy(u => u.Username)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                IsAdmin = u.IsAdmin,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPut("users/{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();
        if (user.IsAdmin) return BadRequest(new { message = "Cannot deactivate admin accounts" });

        user.IsActive = !user.IsActive;
        await _db.SaveChangesAsync();

        return Ok(new { message = user.IsActive ? "User activated" : "User deactivated" });
    }

    [HttpGet("auctions")]
    public async Task<ActionResult<List<AuctionListDto>>> GetAuctions()
    {
        var auctions = await _db.Auctions
            .Include(a => a.Creator)
            .Include(a => a.Bids)
            .OrderByDescending(a => a.EndDate)
            .Select(a => new AuctionListDto
            {
                Id = a.Id,
                Title = a.Title,
                ImageUrl = a.ImageUrl,
                StartingPrice = a.StartingPrice,
                CurrentHighestBid = a.CurrentHighestBid,
                EndDate = a.EndDate,
                IsOpen = a.EndDate > DateTime.UtcNow && a.IsActive,
                BidCount = a.Bids.Count,
                CreatorUsername = a.Creator.Username
            })
            .ToListAsync();

        return Ok(auctions);
    }

    [HttpPut("auctions/{id}/deactivate")]
    public async Task<IActionResult> DeactivateAuction(Guid id)
    {
        var auction = await _db.Auctions.FindAsync(id);
        if (auction == null) return NotFound();

        auction.IsActive = !auction.IsActive;
        await _db.SaveChangesAsync();

        return Ok(new { message = auction.IsActive ? "Auction activated" : "Auction deactivated" });
    }
}
