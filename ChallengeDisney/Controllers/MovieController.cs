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

namespace ChallengeDisney.Controllers
{
    [ApiController]
    [Route("movies")]
    [Authorize]
    public class MovieController : ControllerBase
    {
        private readonly ChallengeDisneyContext _challengeDisneyContext;

        public MovieController(ChallengeDisneyContext ctx)
        {
            _challengeDisneyContext = ctx;
        }

        [HttpGet]
        public List<Movie> GetMovies(string name, int genreId, string order)
        {
            IQueryable<Movie> list = _challengeDisneyContext.Movies.Include(x => x.Characters);

            if (name != null) list = list.Where(x => x.Title == name);

            if (genreId != 0) list = list.Where(x => x.Genre.Id == genreId);

            if (order == "ASC") list = list.OrderBy(x => x.CreationDate);

            if (order == "DESC") list = list.OrderByDescending(x => x.CreationDate);            

            return list.Select(x => new Movie
            {
                Image = x.Image, 
                Title = x.Title, 
                CreationDate = x.CreationDate 
            }).ToList();
        }                

        [HttpGet("details")]        
        public IActionResult GetMovie(string title)
        {
            var movie = _challengeDisneyContext.Movies.Include(x => x.Characters).Include(x => x.Genre).FirstOrDefault(x => x.Title == title);

            if (movie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,"The movie entered does not exist.");
            }

            return StatusCode(StatusCodes.Status200OK, movie);
        }

        [HttpPost("add")]
        public IActionResult Post(MovieRequestModel movie)
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
                var genre = _challengeDisneyContext.Genres.FirstOrDefault(x => x.Id == movie.GenreId);

                if (genre != null)
                {
                    if (newMovie.Genre == null) newMovie.Genre = new Genre();

                    newMovie.Genre = genre;                    
                }
            }

            if (movie.CharacterId != 0)
            {
                var character = _challengeDisneyContext.Characters.FirstOrDefault(x => x.Id == movie.CharacterId);

                if (character != null)
                {
                    if (newMovie.Characters == null) newMovie.Characters = new List<Character>();

                    newMovie.Characters.Add(character);
                }
            }

            _challengeDisneyContext.Movies.Add(newMovie);

            _challengeDisneyContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created, new MovieResponseModel
            {
                Image = movie.Image,
                Title = movie.Title,
                CreationDate = movie.CreationDate,
                Qualification = movie.Qualification,
                GenreId = movie.GenreId
                
            });
        }

        [HttpPut("update")]
        public IActionResult Put(MovieUpdateRequestModel movie)
        {
            var newMovie = _challengeDisneyContext.Movies.Include(x => x.Genre).FirstOrDefault(x => x.Id == movie.Id);

            if (newMovie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The movie entered does not exist.");
            }

            newMovie.Image = movie.Image;
            newMovie.Title = movie.Title;
            newMovie.CreationDate = movie.CreationDate;
            newMovie.Qualification = movie.Qualification;
            newMovie.Genre.Id = movie.GenreId;

            _challengeDisneyContext.Movies.Update(newMovie);

            _challengeDisneyContext.SaveChanges();

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

        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            var delMovie = _challengeDisneyContext.Movies.Find(id);

            if (delMovie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The movie you want to delete does not exist.");
            }

            _challengeDisneyContext.Movies.Remove(delMovie);

            _challengeDisneyContext.SaveChanges();

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
