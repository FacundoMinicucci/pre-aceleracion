using System;
using System.ComponentModel.DataAnnotations;

namespace ChallengeDisney.ViewModels.Genre
{
    public class GenreUpdateRequestModel
    {
        [Required(ErrorMessage = "Please enter the genre ID.")]
        [Range(1, int.MaxValue, ErrorMessage = "The genre ID must be greater than zero. ")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the genre name.")]
        [MaxLength(50, ErrorMessage = "You must enter 50 characters maximum.")]
        [MinLength(4, ErrorMessage = "You must enter at least 4 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the genre image.")]
        [MaxLength(50, ErrorMessage = "You must enter 50 characters maximum.")]
        [MinLength(5, ErrorMessage = "You must enter at least 5 characters.")]
        public string Image { get; set; }
        
    }
}
