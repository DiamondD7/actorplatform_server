namespace UserAPI.Models
{
    public class JwtDTO
    {
        public Guid? Id { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? AccessTokenExpiry { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
