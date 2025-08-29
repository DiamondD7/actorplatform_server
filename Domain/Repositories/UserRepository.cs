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

           
            var newUser = new User
            {
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
            if(string.IsNullOrEmpty(email))
            {
                return false;
            }

            var checking = await _context.UsersTable.FirstOrDefaultAsync(x => x.Email == email);

            if(checking == null) //if an existing email is not found then return true
            {
                return true;
            }

            return false; //if an existing email found, then return false

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
