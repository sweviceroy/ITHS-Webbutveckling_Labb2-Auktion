using Microsoft.EntityFrameworkCore;
using AuctionApi.Data;
using AuctionApi.Models;

namespace AuctionApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuctionDbContext _db;

    public UserRepository(AuctionDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _db.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }

    public async Task CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        await _db.SaveChangesAsync();
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _db.Users.OrderBy(u => u.Username).ToListAsync();
    }
}
