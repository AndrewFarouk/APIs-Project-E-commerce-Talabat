using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.Dtos
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).*$",
            ErrorMessage = "Invalid password format. " +
            "Password must contain at least one uppercase letter, " +
            "one lowercase letter, one digit, and one special character (e.g., @, $, !).")]
        public string Password { get; set; }
    }
}
