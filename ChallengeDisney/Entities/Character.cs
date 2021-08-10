using System.Collections.Generic;

namespace ChallengeDisney.Entities
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }        
        public int Age { get; set; }
        public float Weight { get; set; }
        public string History { get; set; }
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
        
    }
}
