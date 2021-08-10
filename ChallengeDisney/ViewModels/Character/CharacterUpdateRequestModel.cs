using System;
using System.ComponentModel.DataAnnotations;

namespace ChallengeDisney.ViewModels.Character
{
    public class CharacterUpdateRequestModel
    {
        [Required(ErrorMessage = "Please enter the caracted ID.")]
        [Range(1, int.MaxValue, ErrorMessage = "Character ID must be greater than zero.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the character name.")]
        [MaxLength(50, ErrorMessage = "You must enter 50 characters maximum.")]
        [MinLength(4, ErrorMessage = "You must enter at least 4 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the character image.")]
        [MaxLength(50, ErrorMessage = "You must enter 50 characters maximum.")]
        [MinLength(5, ErrorMessage = "You must enter at least 5 characters.")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Please enter the character age.")]
        [Range(1, int.MaxValue, ErrorMessage = "The entered age must be greater than zero")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Please enter the character weight.")]
        [Range(0.1, float.MaxValue, ErrorMessage = "The weight entered cannot be less than zero")]
        public float Weight { get; set; }

        [Required(ErrorMessage = "Please enter the character history.")]
        [MaxLength(255, ErrorMessage = "You must enter 255 characters maximum.")]
        public string History { get; set; }

        [Required(ErrorMessage = "Please enter the movie ID.")]
        [Range(1, int.MaxValue, ErrorMessage = "Movie Id must be greater than zero.")]
        public int MovieId { get; set; }
        
    }
}
