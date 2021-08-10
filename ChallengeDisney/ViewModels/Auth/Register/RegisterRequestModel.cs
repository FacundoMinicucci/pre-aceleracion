using System.ComponentModel.DataAnnotations;

namespace ChallengeDisney.ViewModels.Auth.Register
{
    public class RegisterRequestModel
    {
        [Required(ErrorMessage = "Please enter a username.")]
        [MinLength(6, ErrorMessage = "Your username must have at least 6 characters.")]
        [MaxLength(20, ErrorMessage = "Your username must have 20 characters maximum.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter an email address.")]
        [MinLength(5, ErrorMessage = "Your email address must have more than 5 characters.")]
        [MaxLength(256, ErrorMessage = "Your email address must have fewer than 256 characters.")]
        [EmailAddress(ErrorMessage = "The email address you entered is not valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Your password must have at least 8 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [MinLength(8, ErrorMessage = "Your password must have at least 8 characters.")]
        [Compare("Password", ErrorMessage = "The password does not match.")]
        public string Password2 { get; set; }
    }
}
