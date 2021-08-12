using ChallengeDisney.Context;
using ChallengeDisney.Controllers;
using ChallengeDisney.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace Challenge.Disney.UnitTests.Controller
{
    public class MovieControllerShould
    {        
        private MovieController _movieController;              
        
        [Fact]        
        public async Task Get_With_Invalid_Title_Returns_NotFound()
        {     
            // Arrange
            var options = new DbContextOptionsBuilder<ChallengeDisneyContext>()
               .UseInMemoryDatabase("ChallengeDisneyDatabase")
               .Options;

            await using var ctx = new ChallengeDisneyContext(options);

            await ctx.Movies.AddAsync(new Movie { Title = "Cruella" });
            await ctx.SaveChangesAsync();            

            _movieController = new MovieController(ctx);
            
            // Act
            var value1 = _movieController.GetMovie("Pokémon");
            var result1 = value1 as ObjectResult;

            var value2 = _movieController.GetMovie("Matilda");
            var result2 = value2 as ObjectResult;           

            // Assert           
            Assert.Equal(404, result1.StatusCode);
            Assert.Equal(404, result2.StatusCode);            
        }

        [Fact]
        public async Task Get_WithValidTitle_Returns_Ok()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ChallengeDisneyContext>()
               .UseInMemoryDatabase("ChallengeDisneyDatabase")
               .Options;

            await using var ctx = new ChallengeDisneyContext(options);

            await ctx.Movies.AddAsync(new Movie { Title = "Cruella" });
            await ctx.SaveChangesAsync();

            await ctx.Movies.AddAsync(new Movie { Title = "ToyStory" });
            await ctx.SaveChangesAsync();

            _movieController = new MovieController(ctx);

            // Act
            var value1 = _movieController.GetMovie("Cruella");
            var result1 = value1 as ObjectResult;
            var movie1 = result1.Value as Movie;

            var value2 = _movieController.GetMovie("ToyStory");
            var result2 = value2 as ObjectResult;
            var movie2 = result2.Value as Movie;

            // Assert           
            Assert.Equal("Cruella", movie1.Title);
            Assert.Equal(200, result1.StatusCode);

            Assert.Equal("ToyStory", movie2.Title);
            Assert.Equal(200, result2.StatusCode);

        }               
    }   
}
