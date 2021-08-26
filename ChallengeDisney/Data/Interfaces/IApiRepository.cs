using ChallengeDisney.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChallengeDisney.Interfaces
{
    public interface IApiRepository
    {
        public void Add<T>(T entity) where T : class;

        public void Delete<T>(T entity) where T : class;

        public void Update<T>(T entity) where T : class;

        Task<IEnumerable<Genre>> GetAllGenresAsync();

        Task<Genre> GetGenreByIdAsync(int id);

        Task<IEnumerable<Movie>> GetAllMoviesAsync(string name, int genreId, string order);

        Task<Movie> GetMovieByTitle(string title);

        Task<IEnumerable<Character>> GetAllCharactersAsync(int movieId, int age, string name, float weight);

        Task<Character> GetCharacterByName(string name);
    }
}
