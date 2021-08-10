using System.ComponentModel.DataAnnotations;

namespace ChallengeDisney.ViewModels.Auth.Login
{
    public class LoginRequestModel
    {
        [Required(ErrorMessage = "Please enter your username.")]        
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        public string Password { get; set; }
    }
}
