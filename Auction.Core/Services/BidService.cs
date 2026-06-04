using Auction.Core.DTOs;
using Auction.Core.Interfaces;
using Auction.Data.Interfaces;
using Auction.Domain.Entities;

namespace Auction.Core.Services;

public class BidService : IBidService
{
    private readonly IAuctionRepository _auctions;
    private readonly IBidRepository _bids;
    private readonly IUserRepository _users;

    public BidService(IAuctionRepository auctions, IBidRepository bids, IUserRepository users)
    {
        _auctions = auctions; _bids = bids; _users = users;
    }

    public async Task<BidDto> PlaceBidAsync(Guid auctionId, decimal amount, Guid bidderId)
    {
        var auction = await _auctions.GetByIdAsync(auctionId)
            ?? throw new KeyNotFoundException();

        if (!auction.IsActive || auction.EndDate <= DateTime.UtcNow)
            throw new InvalidOperationException("Auction is closed");

        if (auction.CreatorId == bidderId)
            throw new InvalidOperationException("You cannot bid on your own auction");

        var existingBids = await _bids.GetBidsByAuctionAsync(auctionId);
        var highestBid = existingBids.MaxBy(b => b.Amount);
        var minimumBid = highestBid?.Amount ?? auction.StartingPrice;

        if (amount <= minimumBid)
            throw new InvalidOperationException($"Bid must be higher than {minimumBid}");

        var bid = new Bid { Amount = amount, AuctionId = auctionId, BidderId = bidderId };
        await _bids.CreateAsync(bid);
        auction.CurrentHighestBid = amount;
        await _auctions.UpdateAsync(auction);

        var bidder = (await _users.GetByIdAsync(bidderId))!;
        return new BidDto { Id = bid.Id, Amount = bid.Amount, BidTime = bid.BidTime,
            BidderUsername = bidder.Username, BidderId = bidder.Id };
    }

    public async Task<List<BidDto>> GetBidsAsync(Guid auctionId)
    {
        var bids = await _bids.GetBidsByAuctionAsync(auctionId);
        return bids.Select(b => new BidDto { Id = b.Id, Amount = b.Amount, BidTime = b.BidTime,
            BidderUsername = b.Bidder.Username, BidderId = b.BidderId }).ToList();
    }

    public async Task UndoLatestBidAsync(Guid auctionId, Guid userId)
    {
        var auction = await _auctions.GetByIdAsync(auctionId)
            ?? throw new KeyNotFoundException();

        if (!auction.IsActive || auction.EndDate <= DateTime.UtcNow)
            throw new InvalidOperationException("Auction is closed");

        var latestBid = await _bids.GetLatestBidAsync(auctionId)
            ?? throw new InvalidOperationException("No bids to undo");

        if (latestBid.BidderId != userId)
            throw new InvalidOperationException("You can only undo your own latest bid");

        await _bids.DeleteAsync(latestBid);
        var remainingBids = await _bids.GetBidsByAuctionAsync(auctionId);
        auction.CurrentHighestBid = remainingBids
            .Where(b => b.Id != latestBid.Id)
            .MaxBy(b => (decimal?)b.Amount)?.Amount;
        await _auctions.UpdateAsync(auction);
    }
}
