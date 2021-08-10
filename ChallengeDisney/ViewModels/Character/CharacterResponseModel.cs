namespace ChallengeDisney.ViewModels.Character
{
    public class CharacterResponseModel
    {       
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }       
        public int Age { get; set; }
        public float Weight { get; set; }
        public string History { get; set; }
        public int MovieId { get; set; } 
    }
}
