using System;


namespace ChallengeDisney.ViewModels.Movie
{
    public class MovieResponseModel
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }       
        public int Qualification { get; set; }     
        public int GenreId { get; set; }      
    }
}
