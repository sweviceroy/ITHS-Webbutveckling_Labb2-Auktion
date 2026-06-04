using Auction.Domain.Entities;

namespace Auction.Data.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<List<User>> GetAllAsync();
}
