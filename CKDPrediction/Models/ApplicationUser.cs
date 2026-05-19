using Microsoft.AspNetCore.Identity;

namespace CKDPrediction.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Role { get; set; }
    }
}