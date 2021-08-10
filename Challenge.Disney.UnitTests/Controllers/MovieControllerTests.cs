using ChallengeDisney.Context;
using ChallengeDisney.Controllers;
using ChallengeDisney.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace Challenge.Disney.UnitTests.Controller
{
    public class MovieControllerTests
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
            var value = _movieController.GetMovie("Pokemon");            

            // Assert
            Assert.IsType<NotFoundObjectResult>(value); 
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

            _movieController = new MovieController(ctx);

            // Act
            var value = _movieController.GetMovie("Cruella");

            var result = value as OkObjectResult;

            var movie = result.Value as Movie;

            // Assert
            Assert.IsType<OkObjectResult>(value);
            Assert.Equal("Cruella", movie.Title);
        }
    }   
}
