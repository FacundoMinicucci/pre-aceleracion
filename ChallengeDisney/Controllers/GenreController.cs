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
using ChallengeDisney.Data.UnitOfWork;

namespace ChallengeDisney.Controllers
{
    [ApiController]
    [Route("genres")]
    [Authorize]
    public class GenreController : ControllerBase
    {
        private readonly ChallengeDisneyContext _challengeDisneyContext;
        private readonly IUnitOfWork _unitOfWork;

        public GenreController(ChallengeDisneyContext ctx, IUnitOfWork unitOfWork)
        {
            _challengeDisneyContext = ctx;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetGenresAsync()
        {
            var genres = await _unitOfWork.ApiRepository.GetAllGenresAsync();
            return Ok(genres.Select(x => new { Image = x.Image, Name = x.Name }));
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetGenre(int id)
        {
            var genre = await _unitOfWork.ApiRepository.GetGenreByIdAsync(id);        

            if (genre == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "The genre entered does not exist.");
            }           

            return StatusCode(StatusCodes.Status200OK, genre);
        }
        
        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
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

            _unitOfWork.ApiRepository.Add(newGenre);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return StatusCode(StatusCodes.Status201Created, new GenreResponseModel
                {
                    Name = newGenre.Name,
                    Image = newGenre.Image
                });
            }

            return BadRequest();                             
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(GenreUpdateRequestModel genre)
        {
            var newGenre = await _unitOfWork.ApiRepository.GetGenreByIdAsync(genre.Id);

            if (newGenre == null) return StatusCode(StatusCodes.Status404NotFound, "The genre entered does not exist.");
            
            newGenre.Id = genre.Id;
            newGenre.Image = genre.Image;
            newGenre.Name = genre.Name;

            _unitOfWork.ApiRepository.Update(newGenre);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return StatusCode(StatusCodes.Status201Created, new GenreUpdateResponseModel
                {
                    Id = newGenre.Id,
                    Image = newGenre.Image,
                    Name = newGenre.Name
                });
            }

            return BadRequest();                          
        }

        [HttpDelete("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var delGenre = await _unitOfWork.ApiRepository.GetGenreByIdAsync(id);

            if(delGenre == null) return StatusCode(StatusCodes.Status404NotFound, "The genre you want to delete does not exist.");

            _unitOfWork.ApiRepository.Delete(delGenre);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return BadRequest();           
        }
    }
}
