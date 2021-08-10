using System;
using System.Collections.Generic;

namespace ChallengeDisney.Entities
{
    public class Movie
    {
       
        public int Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }        
        public int Qualification { get; set; }
        public Genre Genre { get; set; }
        public ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
