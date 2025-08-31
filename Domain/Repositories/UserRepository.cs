using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UserAPI.Data;
using UserAPI.Domain.Interfaces;
using UserAPI.Models;
using UserAPI.Services.Jwt;

namespace UserAPI.Domain.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        public UserRepository(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<User> GetTheUserDataAsync(Guid id)
        {
            return await _context.UsersTable.FindAsync(id);
        }

        public async Task<JwtDTO> GenerateTokenAsync(User user)
        {
            JwtService jwtService = new JwtService(_config);
            var generateTokens = jwtService.GenerateTokens(user);

            if(generateTokens == null)
            {
                return null;
            }

            var userDetails = await _context.UsersTable.FirstOrDefaultAsync(x => x.Id == user.Id);
            userDetails.RefreshToken = generateTokens.RefreshToken;
            userDetails.RefreshExpiryDate = generateTokens.RefreshTokenExpiry;
            _context.Entry(userDetails).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return new JwtDTO
            {
                AccessToken = generateTokens.AccessToken,
                RefreshToken = generateTokens.RefreshToken,
                AccessTokenExpiry = generateTokens.AccessTokenExpiry,
            };
        }

        public async Task<bool>CreateUserAsync(User user)
        {
            if(user == null)
            {
                return false;
            }

            string guid = Guid.NewGuid().ToString("N");
            string googleUserName = $"a{guid.Substring(0, 15)}";
           
            var newUser = new User
            {
                UserName = user.AuthProvider == "google" ? googleUserName : user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Password = user.AuthProvider == "google" ? "" : HashPassword(user.Password),
                ProviderId = user.AuthProvider == "google" ? user.ProviderId : "",
                AuthProvider = user.AuthProvider,
            };

            _context.UsersTable.Add(newUser);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<User> CheckLoginCredentials(User user)
        {
            var retrieveUser = await _context.UsersTable
                .FirstOrDefaultAsync(x => x.Email == user.Email);

            if (retrieveUser == null)
            {
                return null; //if user not found, return null
            }

            if (retrieveUser.AuthProvider == "google")
            {
                //if the user is logged in using google, then return the user without checking password
                return retrieveUser;
            }

            if(retrieveUser.Password != HashPassword(user.Password))
            {
                return null; //if password does not match, return null
            }

            return retrieveUser; //if user found and password matches, return the user
        }

        public async Task<bool> CheckExistingEmailAsync(string email)
        {

            var checking = await _context.UsersTable.FirstOrDefaultAsync(x => x.Email == email);

            if(checking == null) //if an existing email is not found then return true
            {
                return true;
            }

            return false; //if an existing email found, then return false

        }

        public async Task<bool> CheckExistingUserNameAsync(string userName)
        {

            var checking = await _context.UsersTable.FirstOrDefaultAsync(x => x.UserName == userName);

            if(checking == null)
            {
                return true;
            }

            return false;
        }

        public async Task<string> UploadProfilePictureAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile-pictures");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            return $"/profile-pictures/{uniqueFileName}";
        }

        public async Task<bool> UpdateUserDataAsync(User user)
        {
            var getUser = await _context.UsersTable.FindAsync(user.Id);


            if (!string.IsNullOrEmpty(user.Gender))
            {
                getUser.Gender = user.Gender;
            }

            if (!string.IsNullOrEmpty(user.Password))
            {
                var hashedPW = HashPassword(user.Password);
                getUser.Password = hashedPW;
            }

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                getUser.ProfilePictureUrl = user.ProfilePictureUrl;
            }

            if (!string.IsNullOrEmpty(user.Bio))
            {
                getUser.Bio = user.Bio;
            }

            if (!string.IsNullOrEmpty(user.UserName))
            {
                var isUserNameUnique = await CheckExistingUserNameAsync(user.UserName);
                if(isUserNameUnique == false)
                {
                    return false;
                }
                getUser.UserName = user.UserName;
            }

            _context.Entry(getUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }


        public string HashPassword(string password)
        {
            SHA256 hash = SHA256.Create();
            var passwordBytes = Encoding.Default.GetBytes(password);
            var hashedPassword = hash.ComputeHash(passwordBytes);

            return Convert.ToHexString(hashedPassword);
        }
    }
}
