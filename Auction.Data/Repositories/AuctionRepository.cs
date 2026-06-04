using Microsoft.EntityFrameworkCore;
using Auction.Data.Interfaces;
using Auction.Domain.Entities;

namespace Auction.Data.Repositories;

public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _db;

    public AuctionRepository(AuctionDbContext db) => _db = db;

    public async Task<AuctionEntity> CreateAsync(AuctionEntity auction)
    {
        _db.Auctions.Add(auction);
        await _db.SaveChangesAsync();
        return auction;
    }

    public async Task<List<AuctionEntity>> GetAllAsync(string? title, bool includeClosed)
    {
        var query = _db.Auctions
            .Include(a => a.Creator)
            .Include(a => a.Bids)
            .Where(a => a.IsActive);

        if (!includeClosed)
            query = query.Where(a => a.EndDate > DateTime.UtcNow);

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(a => a.Title.Contains(title));

        return await query.OrderByDescending(a => a.EndDate).ToListAsync();
    }

    public async Task<AuctionEntity?> GetByIdAsync(Guid id)
    {
        return await _db.Auctions.Include(a => a.Creator).FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<AuctionEntity?> GetByIdWithBidsAsync(Guid id)
    {
        return await _db.Auctions
            .Include(a => a.Creator)
            .Include(a => a.Bids).ThenInclude(b => b.Bidder)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task UpdateAsync(AuctionEntity auction) => await _db.SaveChangesAsync();

    public async Task<List<AuctionEntity>> GetAllForAdminAsync()
    {
        return await _db.Auctions
            .Include(a => a.Creator)
            .Include(a => a.Bids)
            .OrderByDescending(a => a.EndDate)
            .ToListAsync();
    }

    public async Task<AuctionEntity?> GetForAdminAsync(Guid id)
    {
        return await _db.Auctions.Include(a => a.Bids).FirstOrDefaultAsync(a => a.Id == id);
    }
}
