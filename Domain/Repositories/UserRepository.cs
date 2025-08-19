using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UserAPI.Data;
using UserAPI.Domain.Interfaces;
using UserAPI.Models;

namespace UserAPI.Domain.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
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
