using System;
using System.ComponentModel.DataAnnotations;

namespace ChallengeDisney.ViewModels.Movie
{
    public class MovieUpdateRequestModel
    {
        [Required(ErrorMessage = "Please enter the movie ID.")]
        [Range(1, int.MaxValue, ErrorMessage = "Movie Id must be greater than zero.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the movie image.")]
        [MaxLength(50, ErrorMessage = "You must enter 50 characters maximum.")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Please enter the movie title.")]
        [MaxLength(50, ErrorMessage = "You must enter 50 characters maximum.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter the movie creation date.")]
        [DataType(DataType.DateTime, ErrorMessage = "Please enter a valid date.")]
        public DateTime CreationDate { get; set; }

        [Required(ErrorMessage = "Please enter the movie rating.")]
        [Range(1, 5, ErrorMessage = "The movie rating must have a value between 1 and 5.")]
        public int Qualification { get; set; }

        [Required(ErrorMessage = "Please enter the genre ID.")]
        [Range(1, int.MaxValue, ErrorMessage = "Genre ID must be greater than zero.")]
        public int GenreId { get; set; }        
        
    }
}
