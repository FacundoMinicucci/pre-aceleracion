using ChallengeDisney.Context;
using ChallengeDisney.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ChallengeDisney.ViewModels.Genre;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ChallengeDisney.Interfaces;
using System.Threading.Tasks;

namespace ChallengeDisney.Controllers
{
    [ApiController]
    [Route("genres")]
    [Authorize]
    public class GenreController : ControllerBase
    {
        private readonly ChallengeDisneyContext _challengeDisneyContext;
        private readonly IApiRepository _repository;

        public GenreController(ChallengeDisneyContext ctx, IApiRepository repository)
        {
            _challengeDisneyContext = ctx;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetGenresAsync()
        {
            var genres = await _repository.GetAllGenresAsync();
            return Ok(genres.Select(x => new { Image = x.Image, Name = x.Name }));
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetGenre(int id)
        {
            var genre = await _repository.GetGenreByIdAsync(id);        

            if (genre == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The genre entered does not exist.");
            }           

            return StatusCode(StatusCodes.Status200OK, genre);
        }
        
        [HttpPost("add")]
        public async Task<IActionResult> Post(GenreRequestModel genre)
        {
            var newGenre = new Genre
            {
                Name = genre.Name,
                Image = genre.Image
            };

            if (genre.MovieId != 0)
            {                
                var movie = await _challengeDisneyContext.Movies.FirstOrDefaultAsync(x => x.Id == genre.MovieId);

                if (movie != null)
                {
                    if (newGenre.Movies == null) newGenre.Movies = new List<Movie>();

                    newGenre.Movies.Add(movie);
                }
            }

            _repository.Add(newGenre);

            if (await _repository.SaveAll())
            {
                return StatusCode(StatusCodes.Status201Created, new GenreResponseModel
                {
                    Name = newGenre.Name,
                    Image = newGenre.Image
                });
            }

            return StatusCode(StatusCodes.Status400BadRequest);

            
        }

        [HttpPut("update")]
        public async Task<IActionResult> Put(GenreUpdateRequestModel genre)
        {
            var newGenre = await _repository.GetGenreByIdAsync(genre.Id);

            if (newGenre == null) return StatusCode(StatusCodes.Status404NotFound, "The genre entered does not exist.");
            
            newGenre.Id = genre.Id;
            newGenre.Image = genre.Image;
            newGenre.Name = genre.Name;

            _repository.Update(newGenre);

            if (await _repository.SaveAll())
            {
                return StatusCode(StatusCodes.Status201Created, new GenreUpdateResponseModel
                {
                    Id = newGenre.Id,
                    Image = newGenre.Image,
                    Name = newGenre.Name
                });
            }

            return StatusCode(StatusCodes.Status400BadRequest);            
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var delGenre = await _repository.GetGenreByIdAsync(id);

            if(delGenre == null) return StatusCode(StatusCodes.Status404NotFound, "The genre you want to delete does not exist.");
            
            _repository.Delete(delGenre);

            if (await _repository.SaveAll())
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }            

            return StatusCode(StatusCodes.Status400BadRequest);
        }
    }
}
