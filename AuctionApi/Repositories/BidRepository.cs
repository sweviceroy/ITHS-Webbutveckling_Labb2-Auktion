using Microsoft.EntityFrameworkCore;
using AuctionApi.Data;
using AuctionApi.Models;

namespace AuctionApi.Repositories;

public class BidRepository : IBidRepository
{
    private readonly AuctionDbContext _db;

    public BidRepository(AuctionDbContext db)
    {
        _db = db;
    }

    public async Task<Bid> CreateAsync(Bid bid)
    {
        _db.Bids.Add(bid);
        await _db.SaveChangesAsync();
        return bid;
    }

    public async Task<List<Bid>> GetBidsByAuctionAsync(Guid auctionId)
    {
        return await _db.Bids
            .Include(b => b.Bidder)
            .Where(b => b.AuctionId == auctionId)
            .OrderByDescending(b => b.BidTime)
            .ToListAsync();
    }

    public async Task<Bid?> GetLatestBidAsync(Guid auctionId)
    {
        return await _db.Bids
            .Where(b => b.AuctionId == auctionId)
            .OrderByDescending(b => b.BidTime)
            .FirstOrDefaultAsync();
    }

    public async Task DeleteAsync(Bid bid)
    {
        _db.Bids.Remove(bid);
        await _db.SaveChangesAsync();
    }
}
