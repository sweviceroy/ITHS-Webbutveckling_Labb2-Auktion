using Microsoft.EntityFrameworkCore;
using AuctionApi.Data;
using AuctionApi.DTOs;
using AuctionApi.Models;

namespace AuctionApi.Repositories;

public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _db;

    public AuctionRepository(AuctionDbContext db)
    {
        _db = db;
    }

    public async Task<Auction> CreateAsync(Auction auction)
    {
        _db.Auctions.Add(auction);
        await _db.SaveChangesAsync();
        return auction;
    }

    public async Task<List<AuctionListDto>> GetAllAsync(string? title, bool includeClosed)
    {
        var query = _db.Auctions
            .Include(a => a.Creator)
            .Include(a => a.Bids)
            .Where(a => a.IsActive);

        if (!includeClosed)
            query = query.Where(a => a.EndDate > DateTime.UtcNow);

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(a => a.Title.Contains(title));

        return await query
            .OrderByDescending(a => a.EndDate)
            .Select(a => new AuctionListDto
            {
                Id = a.Id,
                Title = a.Title,
                ImageUrl = a.ImageUrl,
                StartingPrice = a.StartingPrice,
                CurrentHighestBid = a.CurrentHighestBid,
                EndDate = a.EndDate,
                IsOpen = a.EndDate > DateTime.UtcNow,
                BidCount = a.Bids.Count,
                CreatorUsername = a.Creator.Username
            })
            .ToListAsync();
    }

    public async Task<Auction?> GetByIdAsync(Guid id)
    {
        return await _db.Auctions
            .Include(a => a.Creator)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Auction?> GetByIdWithBidsAsync(Guid id)
    {
        return await _db.Auctions
            .Include(a => a.Creator)
            .Include(a => a.Bids)
                .ThenInclude(b => b.Bidder)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task UpdateAsync(Auction auction)
    {
        await _db.SaveChangesAsync();
    }

    public async Task<List<AuctionListDto>> GetAllForAdminAsync()
    {
        return await _db.Auctions
            .Include(a => a.Creator)
            .Include(a => a.Bids)
            .OrderByDescending(a => a.EndDate)
            .Select(a => new AuctionListDto
            {
                Id = a.Id,
                Title = a.Title,
                ImageUrl = a.ImageUrl,
                StartingPrice = a.StartingPrice,
                CurrentHighestBid = a.CurrentHighestBid,
                EndDate = a.EndDate,
                IsOpen = a.EndDate > DateTime.UtcNow && a.IsActive,
                IsActive = a.IsActive,
                BidCount = a.Bids.Count,
                CreatorUsername = a.Creator.Username
            })
            .ToListAsync();
    }

    public async Task<Auction?> GetForAdminAsync(Guid id)
    {
        return await _db.Auctions
            .Include(a => a.Bids)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}
