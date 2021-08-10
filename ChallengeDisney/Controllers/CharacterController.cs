using ChallengeDisney.Context;
using ChallengeDisney.Entities;
using ChallengeDisney.ViewModels;
using ChallengeDisney.ViewModels.Character;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ChallengeDisney.Controllers
{
    [ApiController]
    [Route("characters")]
    [Authorize]
    public class CharacterController : ControllerBase
    {
        private readonly ChallengeDisneyContext _challengeDisneyContext;     
        public CharacterController(ChallengeDisneyContext ctx)
        {
            _challengeDisneyContext = ctx;
        }

        [HttpGet]
        public List<Character> GetCharacters(int movieId, int age, string name, float weight)
        {
            IQueryable<Character> list = _challengeDisneyContext.Characters.Include(x => x.Movies);

            if(movieId != 0) list = list.Where(x => x.Movies.FirstOrDefault(x => x.Id == movieId) != null);

            if(age != 0) list = list.Where(x => x.Age == age);

            if(name != null) list = list.Where(x => x.Name == name);

            if(weight != 0) list = list.Where(x => x.Weight == weight);
                                    
            return list.Select(x => new Character 
            {
                Name = x.Name, 
                Image = x.Image
            }).ToList();                        
        }

        [HttpGet("details")]        
        public IActionResult GetCharacter(string name)
        { 
            var character = _challengeDisneyContext.Characters
                .Include(x => x.Movies)                
                .FirstOrDefault(x => x.Name == name);
           
            if (character == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The character entered does not exist.");
            }            

            return StatusCode(StatusCodes.Status200OK, character);
        }

        [HttpPost("add")]        
        public IActionResult Post(CharacterRequestModel character)
        {
            var newCharacter = new Character
            {
                Name = character.Name,
                Image = character.Image,
                Age = character.Age,
                Weight = character.Weight,
                History = character.History               
            };
            
            if (character.MovieId != 0)
            {
                var movie = _challengeDisneyContext.Movies.FirstOrDefault(x => x.Id == character.MovieId);

                if (movie != null)
                {                    
                    if (newCharacter.Movies == null) newCharacter.Movies = new List<Movie>();
                                       
                    newCharacter.Movies.Add(movie);                
                }
            }
            
            _challengeDisneyContext.Characters.Add(newCharacter);

            _challengeDisneyContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created, new CharacterResponseModel
            {
                Id = newCharacter.Id,
                Name = newCharacter.Name,
                Image = newCharacter.Image,
                Age = newCharacter.Age,
                Weight = newCharacter.Weight,
                History = newCharacter.History,                
                MovieId = character.MovieId
            });
        }

        [HttpPut("update")]        
        public IActionResult Put(CharacterUpdateRequestModel character)
        {
            var newCharacter = _challengeDisneyContext.Characters.Include(x => x.Movies).FirstOrDefault(x => x.Id == character.Id);

            if (newCharacter == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The character entered does not exist.");
            }
            
            newCharacter.Id = character.Id;
            newCharacter.Name = character.Name;
            newCharacter.Image = character.Image;
            newCharacter.Age = character.Age;
            newCharacter.Weight = character.Weight;
            newCharacter.History = character.History;           

            _challengeDisneyContext.Characters.Update(newCharacter);

            _challengeDisneyContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created, new CharacterUpdateResponseModel 
            {
                Id = character.Id,
                Name = character.Name,
                Image = character.Image,
                Age = character.Age,
                Weight = character.Weight,
                History = character.History,
                MovieId = character.MovieId
            });
        }

        [HttpDelete("delete")]       
        public IActionResult Delete(int id)
        {
            var delCharacter = _challengeDisneyContext.Characters.Find(id);

            if (delCharacter == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The character you want to delete does not exist.");
            }

            _challengeDisneyContext.Characters.Remove(delCharacter);

            _challengeDisneyContext.SaveChanges();

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
