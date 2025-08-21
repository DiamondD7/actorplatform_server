using System.ComponentModel.DataAnnotations;

namespace UserAPI.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; } = "Prefer not to say";
        public string? Email { get; set; }
        public string? Password { get; set; }

        public string? ProviderId { get; set; }
        public string? AuthProvider { get; set; } = "local"; //this if the acc is made using google or a normal sign up

        public string? RefreshToken { get; set; }
        public DateTime? RefreshExpiryDate { get; set; }

    }
}
