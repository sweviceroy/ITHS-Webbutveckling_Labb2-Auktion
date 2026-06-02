using AuctionApi.DTOs;
using AuctionApi.Models;

namespace AuctionApi.Repositories;

public interface IAuctionRepository
{
    Task<Auction> CreateAsync(Auction auction);
    Task<List<AuctionListDto>> GetAllAsync(string? title, bool includeClosed);
    Task<Auction?> GetByIdAsync(Guid id);
    Task<Auction?> GetByIdWithBidsAsync(Guid id);
    Task UpdateAsync(Auction auction);
    Task<List<AuctionListDto>> GetAllForAdminAsync();
    Task<Auction?> GetForAdminAsync(Guid id);
}
