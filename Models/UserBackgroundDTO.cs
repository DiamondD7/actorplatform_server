namespace UserAPI.Models
{
    public class UserBackgroundDTO
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        public string? Production { get; set; }
        public string? Role { get; set; }
        public string? Director { get; set; }
        public string? Year { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
