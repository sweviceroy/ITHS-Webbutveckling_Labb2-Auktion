using Auction.Core.DTOs;

namespace Auction.Core.Interfaces;

public interface IAdminService
{
    Task<List<UserDto>> GetUsersAsync();
    Task<string> ToggleUserActiveAsync(Guid id);
    Task<List<AuctionListDto>> GetAuctionsAsync();
    Task<string> ToggleAuctionActiveAsync(Guid id);
}
