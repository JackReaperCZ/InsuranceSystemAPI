using InsuranceSystemAPI.DTOs;
using InsuranceSystemAPI.Models;

namespace InsuranceSystemAPI.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
        Task<UserDto?> RegisterAsync(RegisterRequestDto registerRequest);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<bool> IsUserActiveAsync(int userId);
        string GenerateJwtToken(User user);
    }
}