using UserAPI.Models;

namespace UserAPI.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetTheUserDataAsync(Guid id);
        Task<JwtDTO> GenerateTokenAsync(User user);
        Task<bool> CreateUserAsync(User user);
        Task<User> CheckLoginCredentials(User user);
        Task<bool> CheckExistingEmailAsync(string email);
        Task<bool> CheckExistingUserNameAsync(string userName);
        Task<string>UploadProfilePictureAsync(IFormFile file);
        Task<bool> UpdateUserDataAsync(User user);
        string HashPassword(string password);
    }
}
