using ChallengeDisney.Context;
using ChallengeDisney.Entities;
using ChallengeDisney.ViewModels;
using ChallengeDisney.ViewModels.Movie;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ChallengeDisney.Interfaces;
using System.Threading.Tasks;

namespace ChallengeDisney.Controllers
{
    [ApiController]
    [Route("movies")]
    [Authorize]
    public class MovieController : ControllerBase
    {
        private readonly ChallengeDisneyContext _challengeDisneyContext;
        private readonly IApiRepository _repository;

        public MovieController(ChallengeDisneyContext ctx, IApiRepository repository)
        {
            _challengeDisneyContext = ctx;
            _repository = repository;
        }

        [HttpGet]
        public async Task<List<Movie>> GetMovies(string name, int genreId, string order)
        {
            var list = await _repository.GetAllMoviesAsync(name, genreId,order);            

            return list.Select(x => new Movie
            {
                Image = x.Image, 
                Title = x.Title, 
                CreationDate = x.CreationDate 
            }).ToList();
        }                

        [HttpGet("details")]        
        public async Task<IActionResult> GetMovie(string title)
        {
            var movie = await _repository.GetMovieByTitle(title);

            if (movie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The movie entered does not exist.");
            }

            return StatusCode(StatusCodes.Status200OK, movie);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Post(MovieRequestModel movie)
        {
            var newMovie = new Movie
            {
                Image = movie.Image,
                Title = movie.Title,
                CreationDate = movie.CreationDate,
                Qualification = movie.Qualification                
            };

            if (movie.GenreId != 0)
            {
                var genre = await _challengeDisneyContext.Genres.FirstOrDefaultAsync(x => x.Id == movie.GenreId);

                if (genre != null)
                {
                    if (newMovie.Genre == null) newMovie.Genre = new Genre();

                    newMovie.Genre = genre;                    
                }
            }

            if (movie.CharacterId != 0)
            {
                var character = await _challengeDisneyContext.Characters.FirstOrDefaultAsync(x => x.Id == movie.CharacterId);

                if (character != null)
                {
                    if (newMovie.Characters == null) newMovie.Characters = new List<Character>();

                    newMovie.Characters.Add(character);
                }
            }

            _repository.Add(newMovie);

            if (await _repository.SaveAll())
            {
                return StatusCode(StatusCodes.Status201Created, new MovieResponseModel
                {
                    Image = movie.Image,
                    Title = movie.Title,
                    CreationDate = movie.CreationDate,
                    Qualification = movie.Qualification,
                    GenreId = movie.GenreId

                });
            }

            return StatusCode(StatusCodes.Status400BadRequest);            
        }

        [HttpPut("update")]
        public async Task<IActionResult> Put(MovieUpdateRequestModel movie)
        {
            var newMovie = await _challengeDisneyContext.Movies.Include(x => x.Genre).FirstOrDefaultAsync(x => x.Id == movie.Id);

            if (newMovie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The movie entered does not exist.");
            }

            newMovie.Image = movie.Image;
            newMovie.Title = movie.Title;
            newMovie.CreationDate = movie.CreationDate;
            newMovie.Qualification = movie.Qualification;
            newMovie.Genre.Id = movie.GenreId;

            _repository.Update(newMovie);

            if (await _repository.SaveAll())
            {
                return StatusCode(StatusCodes.Status201Created, new MovieUpdateResponseModel
                {
                    Id = newMovie.Id,
                    Image = newMovie.Image,
                    Title = newMovie.Title,
                    CreationDate = newMovie.CreationDate,
                    Qualification = newMovie.Qualification,
                    GenreId = newMovie.Genre.Id
                });
            }

            return StatusCode(StatusCodes.Status400BadRequest);           
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var delMovie = await _challengeDisneyContext.Movies.FindAsync(id);

            if (delMovie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The movie you want to delete does not exist.");
            }

            _repository.Delete(delMovie);

            if (await _repository.SaveAll())
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return StatusCode(StatusCodes.Status400BadRequest);            
        }
    }
}
