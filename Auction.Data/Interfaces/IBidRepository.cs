using Auction.Domain.Entities;

namespace Auction.Data.Interfaces;

public interface IBidRepository
{
    Task<Bid> CreateAsync(Bid bid);
    Task<List<Bid>> GetBidsByAuctionAsync(Guid auctionId);
    Task<Bid?> GetLatestBidAsync(Guid auctionId);
    Task DeleteAsync(Bid bid);
}
