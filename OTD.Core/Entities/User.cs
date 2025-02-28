using System.ComponentModel.DataAnnotations;

namespace OTD.Core.Entities
{
    public class User : BaseEntity
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(101)] public string DisplayName { get; set; }

        [Required]        
        [StringLength(320)]
        public string Email { get; set; }

        public DateTime? LastLoginDate { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        public bool IsEmailConfirmed { get; set; } = false;
    }
}
