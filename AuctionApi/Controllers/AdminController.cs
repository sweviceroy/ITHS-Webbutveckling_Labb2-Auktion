using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuctionApi.DTOs;
using AuctionApi.Repositories;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IAuctionRepository _auctions;

    public AdminController(IUserRepository users, IAuctionRepository auctions)
    {
        _users = users;
        _auctions = auctions;
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _users.GetAllAsync();

        return Ok(users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            IsAdmin = u.IsAdmin,
            CreatedAt = u.CreatedAt
        }).ToList());
    }

    [HttpPut("users/{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null) return NotFound();
        if (user.IsAdmin) return BadRequest(new { message = "Cannot deactivate admin accounts" });

        user.IsActive = !user.IsActive;
        await _users.UpdateAsync(user);

        return Ok(new { message = user.IsActive ? "User activated" : "User deactivated" });
    }

    [HttpGet("auctions")]
    public async Task<ActionResult<List<AuctionListDto>>> GetAuctions()
    {
        var auctions = await _auctions.GetAllForAdminAsync();
        return Ok(auctions);
    }

    [HttpPut("auctions/{id}/deactivate")]
    public async Task<IActionResult> DeactivateAuction(Guid id)
    {
        var auction = await _auctions.GetForAdminAsync(id);
        if (auction == null) return NotFound();

        auction.IsActive = !auction.IsActive;
        await _auctions.UpdateAsync(auction);

        return Ok(new { message = auction.IsActive ? "Auction activated" : "Auction deactivated" });
    }
}
