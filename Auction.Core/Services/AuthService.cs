using Auction.Core.DTOs;
using Auction.Core.Interfaces;
using Auction.Data.Interfaces;

namespace Auction.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly JwtService _jwt;

    public AuthService(IUserRepository users, JwtService jwt)
    {
        _users = users;
        _jwt = jwt;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _users.UsernameExistsAsync(dto.Username))
            throw new InvalidOperationException("Username already taken");

        if (await _users.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Email already in use");

        var user = new Domain.Entities.User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _users.CreateAsync(user);

        return new AuthResponseDto { Token = _jwt.GenerateToken(user), User = MapUserDto(user) };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _users.GetByUsernameAsync(dto.Username);
        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        return new AuthResponseDto { Token = _jwt.GenerateToken(user), User = MapUserDto(user) };
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await _users.GetByIdAsync(userId) ?? throw new KeyNotFoundException();
        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            throw new InvalidOperationException("Current password is incorrect");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _users.UpdateAsync(user);
    }

    public UserDto MapUserDto(Domain.Entities.User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        IsAdmin = user.IsAdmin,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt
    };
}
