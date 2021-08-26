using ChallengeDisney.Context;
using ChallengeDisney.Entities;
using ChallengeDisney.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengeDisney.Data
{
    public class ApiRepository : IApiRepository
    {
        private readonly ChallengeDisneyContext _context;

        public ApiRepository(ChallengeDisneyContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            var genres = await _context.Genres.ToListAsync();

            return genres;

        }

        public async Task<Genre> GetGenreByIdAsync(int id)
        {
            var genre = await _context.Genres
                .Include(x => x.Movies)
                .FirstOrDefaultAsync(x => x.Id == id);

            return genre;
        }        

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync(string name, int genreId, string order)
        {
            IQueryable<Movie> list = _context.Movies.Include(x => x.Characters);

            if (name != null) list = list.Where(x => x.Title == name);

            if (genreId != 0) list = list.Where(x => x.Genre.Id == genreId);

            if (order == "ASC") list = list.OrderBy(x => x.CreationDate);

            if (order == "DESC") list = list.OrderByDescending(x => x.CreationDate);            

            return await list.ToListAsync();
        }

        public async Task<Movie> GetMovieByTitle(string title)
        {
            var movie = await _context.Movies.Include(x => x.Characters).Include(x => x.Genre).FirstOrDefaultAsync(x => x.Title == title);           

            return movie;
        }

        public async Task<IEnumerable<Character>> GetAllCharactersAsync(int movieId, int age, string name, float weight)
        {
            IQueryable<Character> list = _context.Characters.Include(x => x.Movies);

            if (movieId != 0) list = list.Where(x => x.Movies.FirstOrDefault(x => x.Id == movieId) != null);

            if (age != 0) list = list.Where(x => x.Age == age);

            if (name != null) list = list.Where(x => x.Name == name);

            if (weight != 0) list = list.Where(x => x.Weight == weight);

            return await list.ToListAsync();
        }

        public async Task<Character> GetCharacterByName(string name)
        {
            var character = await _context.Characters
                .Include(x => x.Movies)
                .FirstOrDefaultAsync(x => x.Name == name);

            return character;
        }        
    }
}
