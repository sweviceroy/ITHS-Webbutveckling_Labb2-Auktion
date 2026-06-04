using Auction.Core.DTOs;

namespace Auction.Core.Interfaces;

public interface IAuctionService
{
    Task<AuctionDetailDto> CreateAuctionAsync(CreateAuctionDto dto, Guid creatorId);
    Task<List<AuctionListDto>> GetAuctionsAsync(string? title, bool includeClosed);
    Task<AuctionDetailDto?> GetAuctionByIdAsync(Guid id);
    Task<AuctionDetailDto> UpdateAuctionAsync(Guid id, UpdateAuctionDto dto, Guid userId);
    Task<List<AuctionListDto>> GetAllForAdminAsync();
    Task ToggleAuctionActiveAsync(Guid id);
}
