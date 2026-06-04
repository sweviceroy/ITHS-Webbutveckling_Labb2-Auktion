using Auction.Core.DTOs;
using Auction.Core.Interfaces;
using Auction.Data.Interfaces;

namespace Auction.Core.Services;

public class AuctionService : IAuctionService
{
    private readonly IAuctionRepository _auctions;

    public AuctionService(IAuctionRepository auctions) => _auctions = auctions;

    public async Task<AuctionDetailDto> CreateAuctionAsync(CreateAuctionDto dto, Guid creatorId)
    {
        if (dto.EndDate <= dto.StartDate)
            throw new InvalidOperationException("End date must be after start date");

        if (dto.EndDate <= DateTime.UtcNow)
            throw new InvalidOperationException("End date must be in the future");

        var auction = new AuctionEntity
        {
            Title = dto.Title, Description = dto.Description, ImageUrl = dto.ImageUrl,
            StartingPrice = dto.StartingPrice, StartDate = dto.StartDate, EndDate = dto.EndDate,
            CreatorId = creatorId
        };

        await _auctions.CreateAsync(auction);
        var created = (await _auctions.GetByIdAsync(auction.Id))!;
        return MapDetailDto(created);
    }

    public async Task<List<AuctionListDto>> GetAuctionsAsync(string? title, bool includeClosed)
    {
        var auctions = await _auctions.GetAllAsync(title, includeClosed);
        return auctions.Select(MapListDto).ToList();
    }

    public async Task<AuctionDetailDto?> GetAuctionByIdAsync(Guid id)
    {
        var auction = await _auctions.GetByIdWithBidsAsync(id);
        return auction == null ? null : MapDetailDto(auction);
    }

    public async Task<AuctionDetailDto> UpdateAuctionAsync(Guid id, UpdateAuctionDto dto, Guid userId)
    {
        var auction = await _auctions.GetByIdWithBidsAsync(id)
            ?? throw new KeyNotFoundException();

        if (auction.CreatorId != userId)
            throw new UnauthorizedAccessException();

        if (!auction.IsActive || auction.EndDate <= DateTime.UtcNow)
            throw new InvalidOperationException("Cannot update a closed auction");

        auction.Title = dto.Title;
        auction.Description = dto.Description;
        auction.ImageUrl = dto.ImageUrl;
        auction.EndDate = dto.EndDate;

        if (dto.StartingPrice.HasValue)
        {
            if (auction.Bids.Count > 0)
                throw new InvalidOperationException("Cannot change starting price once bids have been placed");
            auction.StartingPrice = dto.StartingPrice.Value;
        }

        await _auctions.UpdateAsync(auction);
        return MapDetailDto(auction);
    }

    public async Task<List<AuctionListDto>> GetAllForAdminAsync()
    {
        var auctions = await _auctions.GetAllForAdminAsync();
        return auctions.Select(MapListDto).ToList();
    }

    public async Task ToggleAuctionActiveAsync(Guid id)
    {
        var auction = await _auctions.GetForAdminAsync(id) ?? throw new KeyNotFoundException();
        auction.IsActive = !auction.IsActive;
        await _auctions.UpdateAsync(auction);
    }

    private static AuctionListDto MapListDto(AuctionEntity a) => new()
    {
        Id = a.Id, Title = a.Title, ImageUrl = a.ImageUrl,
        StartingPrice = a.StartingPrice, CurrentHighestBid = a.CurrentHighestBid,
        EndDate = a.EndDate, IsOpen = a.EndDate > DateTime.UtcNow && a.IsActive,
        IsActive = a.IsActive, BidCount = a.Bids.Count, CreatorUsername = a.Creator.Username
    };

    private static AuctionDetailDto MapDetailDto(AuctionEntity a) => new()
    {
        Id = a.Id, Title = a.Title, Description = a.Description, ImageUrl = a.ImageUrl,
        StartingPrice = a.StartingPrice, CurrentHighestBid = a.CurrentHighestBid,
        StartDate = a.StartDate, EndDate = a.EndDate,
        IsOpen = a.EndDate > DateTime.UtcNow && a.IsActive,
        CreatorId = a.CreatorId, CreatorUsername = a.Creator?.Username ?? string.Empty,
        Bids = a.Bids?.OrderByDescending(b => b.BidTime).Select(b => new BidDto
        {
            Id = b.Id, Amount = b.Amount, BidTime = b.BidTime,
            BidderUsername = b.Bidder?.Username ?? string.Empty, BidderId = b.BidderId
        }).ToList() ?? new()
    };
}
