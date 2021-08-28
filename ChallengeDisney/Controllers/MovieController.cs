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
using ChallengeDisney.Data.UnitOfWork;

namespace ChallengeDisney.Controllers
{
    [ApiController]
    [Route("movies")]
    [Authorize]
    public class MovieController : ControllerBase
    {
        private readonly ChallengeDisneyContext _challengeDisneyContext;
        private readonly IUnitOfWork _unitOfWork;

        public MovieController(ChallengeDisneyContext ctx, IUnitOfWork unitOfWork)
        {
            _challengeDisneyContext = ctx;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<List<Movie>> GetMovies(string name, int genreId, string order)
        {
            var list = await _unitOfWork.ApiRepository.GetAllMoviesAsync(name, genreId,order);            

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
            var movie = await _unitOfWork.ApiRepository.GetMovieByTitle(title);

            if (movie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The movie entered does not exist.");
            }

            return StatusCode(StatusCodes.Status200OK, movie);
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
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

            _unitOfWork.ApiRepository.Add(newMovie);

            if (await _unitOfWork.SaveChangesAsync())
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

            return BadRequest();                          
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
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

            _unitOfWork.ApiRepository.Update(newMovie);

            if (await _unitOfWork.SaveChangesAsync())
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

            return BadRequest();                         
        }

        [HttpDelete("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var delMovie = await _challengeDisneyContext.Movies.FindAsync(id);

            if (delMovie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The movie you want to delete does not exist.");
            }

            _unitOfWork.ApiRepository.Delete(delMovie);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return BadRequest();                                  
        }
    }
}
