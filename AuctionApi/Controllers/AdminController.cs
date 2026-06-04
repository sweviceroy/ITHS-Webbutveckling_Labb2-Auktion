using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auction.Core.DTOs;
using Auction.Core.Interfaces;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _admin;

    public AdminController(IAdminService admin) => _admin = admin;

    [HttpGet("users")]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        return Ok(await _admin.GetUsersAsync());
    }

    [HttpPut("users/{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        try { return Ok(new { message = await _admin.ToggleUserActiveAsync(id) }); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("auctions")]
    public async Task<ActionResult<List<AuctionListDto>>> GetAuctions()
    {
        return Ok(await _admin.GetAuctionsAsync());
    }

    [HttpPut("auctions/{id}/deactivate")]
    public async Task<IActionResult> DeactivateAuction(Guid id)
    {
        try { return Ok(new { message = await _admin.ToggleAuctionActiveAsync(id) }); }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}
