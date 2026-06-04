using Auction.Core.DTOs;

namespace Auction.Core.Interfaces;

public interface IBidService
{
    Task<BidDto> PlaceBidAsync(Guid auctionId, decimal amount, Guid bidderId);
    Task<List<BidDto>> GetBidsAsync(Guid auctionId);
    Task UndoLatestBidAsync(Guid auctionId, Guid userId);
}
