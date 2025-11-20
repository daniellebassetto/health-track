using HealthTrack.Core.Models.Enums;
using HealthTrack.Core.Utils;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HealthTrack.Core.Models.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Patient;

        public DateTime CreatedAt { get; set; } = DateTimeHelper.Now;

        public virtual Patient? Patient { get; set; }
    }
}