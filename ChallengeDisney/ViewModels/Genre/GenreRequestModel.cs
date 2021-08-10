using System.ComponentModel.DataAnnotations;

namespace ChallengeDisney.ViewModels.Genre
{
    public class GenreRequestModel
    {
        [Required(ErrorMessage = "Please enter the genre name.")]
        [MaxLength(50, ErrorMessage = "You must enter 50 characters maximum.")]
        [MinLength(4, ErrorMessage = "You must enter at least 4 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the genre image.")]
        [MaxLength(50, ErrorMessage = "You must enter 50 characters maximum.")]
        [MinLength(5, ErrorMessage = "You must enter at least 5 characters.")]
        public string Image { get; set; }
        
        public int MovieId { get; set; }
    }
}
