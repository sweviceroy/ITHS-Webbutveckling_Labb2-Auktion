namespace Auction.Core.Interfaces;

public interface IAuthService
{
    Task<DTOs.AuthResponseDto> RegisterAsync(DTOs.RegisterDto dto);
    Task<DTOs.AuthResponseDto> LoginAsync(DTOs.LoginDto dto);
    Task ChangePasswordAsync(Guid userId, DTOs.ChangePasswordDto dto);
    DTOs.UserDto MapUserDto(Domain.Entities.User user);
}
