using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InsuranceSystemAPI.Data;
using InsuranceSystemAPI.DTOs;
using InsuranceSystemAPI.Models;

namespace InsuranceSystemAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly InsuranceDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(InsuranceDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

            if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.HashedPassword))
            {
                return null;
            }

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var expiration = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("Jwt:ExpiryInHours"));

            return new LoginResponseDto
            {
                Token = token,
                Expiration = expiration,
                User = MapToUserDto(user)
            };
        }

        public async Task<UserDto?> RegisterAsync(RegisterRequestDto registerRequest)
        {
            // Check if username or email already exists
            var existingUser = await _context.Users
                .AnyAsync(u => u.Username == registerRequest.Username || 
                              u.Email == registerRequest.Email);

            if (existingUser)
            {
                return null;
            }

            var user = new User
            {
                Username = registerRequest.Username,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password),
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Email = registerRequest.Email,
                Phone = registerRequest.Phone,
                Role = registerRequest.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToUserDto(user);
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, user.HashedPassword))
            {
                return false;
            }

            user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user != null ? MapToUserDto(user) : null;
        }

        public async Task<bool> IsUserActiveAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.IsActive ?? false;
        }

        public string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var expiryInHours = _configuration.GetValue<int>("Jwt:ExpiryInHours");

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException("JWT configuration key 'Jwt:Key' is missing or empty.");
            }
            if (string.IsNullOrWhiteSpace(jwtIssuer))
            {
                throw new InvalidOperationException("JWT configuration key 'Jwt:Issuer' is missing or empty.");
            }
            if (string.IsNullOrWhiteSpace(jwtAudience))
            {
                throw new InvalidOperationException("JWT configuration key 'Jwt:Audience' is missing or empty.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("FullName", $"{user.FirstName} {user.LastName}")
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiryInHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin
            };
        }
    }
}