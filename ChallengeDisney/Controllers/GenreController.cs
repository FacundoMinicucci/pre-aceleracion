using ChallengeDisney.Context;
using ChallengeDisney.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ChallengeDisney.ViewModels.Genre;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ChallengeDisney.Controllers
{
    [ApiController]
    [Route("genres")]
    [Authorize]
    public class GenreController : ControllerBase
    {
        private readonly ChallengeDisneyContext _challengeDisneyContext;

        public GenreController(ChallengeDisneyContext ctx)
        {
            _challengeDisneyContext = ctx;
        }

        [HttpGet]
        public IActionResult GetGenres()
        {
            return Ok(_challengeDisneyContext.Genres
                .Select(x => new { Name = x.Name, Image = x.Image })
                .ToList());
        }

        [HttpGet("details")]
        public IActionResult GetGenre(int id)
        {
            var genre = _challengeDisneyContext.Genres
                .Include(x => x.Movies)
                .FirstOrDefault(x => x.Id == id);            

            if (genre == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The genre entered does not exist.");
            }           

            return StatusCode(StatusCodes.Status200OK, genre);
        }
        
        [HttpPost("add")]
        public IActionResult Post(GenreRequestModel genre)
        {
            var newGenre = new Genre
            {
                Name = genre.Name,
                Image = genre.Image
            };

            if (genre.MovieId != 0)
            {
                var movie = _challengeDisneyContext.Movies.FirstOrDefault(x => x.Id == genre.MovieId);

                if (movie != null)
                {
                    if (newGenre.Movies == null) newGenre.Movies = new List<Movie>();

                    newGenre.Movies.Add(movie);
                }
            }

            _challengeDisneyContext.Genres.Add(newGenre);

            _challengeDisneyContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created, new GenreResponseModel
            {
                Name = newGenre.Name,
                Image = newGenre.Image
            });
        }

        [HttpPut("update")]
        public IActionResult Put(GenreUpdateRequestModel genre)
        {
            var newGenre = _challengeDisneyContext.Genres.FirstOrDefault(x => x.Id == genre.Id);

            if (newGenre == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The genre entered does not exist.");
            }

            newGenre.Id = genre.Id;
            newGenre.Image = genre.Image;
            newGenre.Name = genre.Name;

            _challengeDisneyContext.Genres.Update(newGenre);

            _challengeDisneyContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created, new GenreUpdateResponseModel
            {
                Id = newGenre.Id,
                Image = newGenre.Image,
                Name = newGenre.Name               
            });
        }

        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            var delGenre = _challengeDisneyContext.Genres.Find(id);

            if(delGenre == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The genre you want to delete does not exist.");
            } 

            _challengeDisneyContext.Genres.Remove(delGenre);
            _challengeDisneyContext.SaveChanges();

            return StatusCode(StatusCodes.Status204NoContent);

        }
    }
}
