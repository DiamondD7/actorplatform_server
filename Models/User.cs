using System.ComponentModel.DataAnnotations;

namespace UserAPI.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? ProfilePictureUrl { get; set; } = "/profile-pictures/avatar-ungendered.png";
        public string? Bio { get; set; }
        public string? MobileNumber { get; set; }
        public string? UserName { get; set; }
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
        public Appearance? Appearance { get; set; } = new Appearance();
        public PersonalBackground? PersonalBackground { get; set; } = new PersonalBackground();

    }

    public class Appearance
    {
        public int Id { get; set; }
        public string? Height { get; set; } = "";
        public string? Weight { get; set; } = "";
        public string? EyeColor { get; set; } = "";
        public string? HairColor { get; set; } = "";
    }

    public class PersonalBackground
    {
        public int Id { get; set; }
        public string? Ethnicity { get; set; }
        public string? NaturalAccent { get; set; }
    }
}
