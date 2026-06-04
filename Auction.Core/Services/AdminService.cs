using Auction.Core.DTOs;
using Auction.Core.Interfaces;
using Auction.Data.Interfaces;

namespace Auction.Core.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _users;
    private readonly IAuctionRepository _auctions;

    public AdminService(IUserRepository users, IAuctionRepository auctions)
    {
        _users = users;
        _auctions = auctions;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var users = await _users.GetAllAsync();
        return users.Select(u => new UserDto
        {
            Id = u.Id, Username = u.Username, Email = u.Email,
            IsAdmin = u.IsAdmin, IsActive = u.IsActive, CreatedAt = u.CreatedAt
        }).ToList();
    }

    public async Task<string> ToggleUserActiveAsync(Guid id)
    {
        var user = await _users.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        if (user.IsAdmin) throw new InvalidOperationException("Cannot deactivate admin accounts");

        user.IsActive = !user.IsActive;
        await _users.UpdateAsync(user);
        return user.IsActive ? "User activated" : "User deactivated";
    }

    public async Task<List<AuctionListDto>> GetAuctionsAsync()
    {
        var auctions = await _auctions.GetAllForAdminAsync();
        return auctions.Select(a => new AuctionListDto
        {
            Id = a.Id, Title = a.Title, ImageUrl = a.ImageUrl,
            StartingPrice = a.StartingPrice, CurrentHighestBid = a.CurrentHighestBid,
            EndDate = a.EndDate,
            IsOpen = a.EndDate > DateTime.UtcNow && a.IsActive,
            IsActive = a.IsActive, BidCount = a.Bids.Count,
            CreatorUsername = a.Creator.Username
        }).ToList();
    }

    public async Task<string> ToggleAuctionActiveAsync(Guid id)
    {
        var auction = await _auctions.GetForAdminAsync(id) ?? throw new KeyNotFoundException();
        auction.IsActive = !auction.IsActive;
        await _auctions.UpdateAsync(auction);
        return auction.IsActive ? "Auction activated" : "Auction deactivated";
    }
}
