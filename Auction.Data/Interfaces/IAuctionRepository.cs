using Auction.Domain.Entities;

namespace Auction.Data.Interfaces;

public interface IAuctionRepository
{
    Task<AuctionEntity> CreateAsync(AuctionEntity auction);
    Task<List<AuctionEntity>> GetAllAsync(string? title, bool includeClosed);
    Task<AuctionEntity?> GetByIdAsync(Guid id);
    Task<AuctionEntity?> GetByIdWithBidsAsync(Guid id);
    Task UpdateAsync(AuctionEntity auction);
    Task<List<AuctionEntity>> GetAllForAdminAsync();
    Task<AuctionEntity?> GetForAdminAsync(Guid id);
}
