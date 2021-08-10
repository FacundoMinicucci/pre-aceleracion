using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengeDisney.ViewModels.Character
{
    public class CharacterUpdateResponseModel
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
