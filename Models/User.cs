using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public ICollection<UserBackground> UserBackgrounds { get; set; } = new List<UserBackground>();

    }

    public class UserBackground
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string? Title { get; set; }
        public string? Production { get; set; }
        public string? ProductionType { get; set; }
        public string? Role { get; set; }
        public string? Director { get; set; }
        public string? Year { get; set; }
        public string? Note { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool? IsFeatured { get; set; }
        public List<VideoUrls> VideoUrls { get; set; } = new();

    }

    public class VideoUrls
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(UserBackground))]
        public int UserBackgroundId { get; set; }
        public UserBackground UserBackground { get; set; } = null!;
        public string? Url { get; set; }
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
        public string? Ethnicity { get; set; } = "";
        public string? NaturalAccent { get; set; } = "";
    }


}
