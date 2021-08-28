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
using ChallengeDisney.Interfaces;
using System.Threading.Tasks;
using ChallengeDisney.Data.UnitOfWork;

namespace ChallengeDisney.Controllers
{
    [ApiController]
    [Route("characters")]
    [Authorize]
    public class CharacterController : ControllerBase
    {
        private readonly ChallengeDisneyContext _challengeDisneyContext;
        private readonly IUnitOfWork _unitOfWork;
        public CharacterController(ChallengeDisneyContext ctx, IUnitOfWork unitOfWork)
        {
            _challengeDisneyContext = ctx;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<List<Character>> GetCharacters(int movieId, int age, string name, float weight)
        {
            var list = await _unitOfWork.ApiRepository.GetAllCharactersAsync(movieId, age, name, weight);
                                    
            return list.Select(x => new Character 
            {
                Name = x.Name, 
                Image = x.Image
            }).ToList();                        
        }

        [HttpGet("details")]        
        public async Task<IActionResult> GetCharacter(string name)
        {
            var character = await _unitOfWork.ApiRepository.GetCharacterByName(name);
           
            if (character == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The character entered does not exist.");
            }            

            return StatusCode(StatusCodes.Status200OK, character);
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(CharacterRequestModel character)
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
                var movie = await _challengeDisneyContext.Movies.FirstOrDefaultAsync(x => x.Id == character.MovieId);
                
                if (movie != null)
                {                    
                    if (newCharacter.Movies == null) newCharacter.Movies = new List<Movie>();
                                       
                    newCharacter.Movies.Add(movie);                
                }
            }

            _unitOfWork.ApiRepository.Add(newCharacter);

            if (await _unitOfWork.SaveChangesAsync())
            {
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

            return BadRequest();            
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(CharacterUpdateRequestModel character)
        {
            var newCharacter = await _challengeDisneyContext.Characters.Include(x => x.Movies).FirstOrDefaultAsync(x => x.Id == character.Id);

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

            _unitOfWork.ApiRepository.Update(newCharacter);

            if (await _unitOfWork.SaveChangesAsync())
            {
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

            return BadRequest();              
        }

        [HttpDelete("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var delCharacter = await _challengeDisneyContext.Characters.FindAsync(id);

            if (delCharacter == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The character you want to delete does not exist.");
            }

            _unitOfWork.ApiRepository.Delete(delCharacter);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return BadRequest();                                  
        }
    }
}
