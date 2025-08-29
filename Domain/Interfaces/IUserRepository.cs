using UserAPI.Models;

namespace UserAPI.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<JwtDTO> GenerateTokenAsync(User user);
        Task<bool> CreateUserAsync(User user);
        Task<User> CheckLoginCredentials(User user);
        Task<bool> CheckExistingEmailAsync(string email);
        string HashPassword(string password);
    }
}
